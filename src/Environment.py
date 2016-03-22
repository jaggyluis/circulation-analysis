from collections import defaultdict
import rhinoscriptsyntax as rs
import Rhino as rc
import scriptcontext as sc
import random, copy
from itertools import permutations
import math as m
import Grasshopper.Kernel as gh

import ghpythonlib.parallel

class Graph(object):
    
    def __init__(self, nodes=None, edges=None, distances=None):
        self.nodes = nodes if nodes else set()
        self.edges = edges if edges else defaultdict(set)
        self.distances = distances if distances else dict()

    def __repr__(self):
        return "Graph: {0} nodes, {1} edges".format(len(self.nodes), len(self.edges))
    
    def add_node(self, value):
        self.nodes.add(value)
        
    def add_edge(self, from_node, to_node, distance, directed=False):
        self.add_node(from_node)
        self.add_node(to_node)
        
        self.edges[from_node].add(to_node)
        self.distances[(from_node, to_node)] = distance
        if directed:
            self.edges[to_node].add(from_node)
            self.distances[(to_node, from_node)] = distance
    
    def get_distance(self, key, gen=0):
        return self.distances[key]
    
    def get_step(self, initial, goal, route):
        return
    
    def deepsearch(self, key, func, depth=0, visited=None, max_depth=10):
        if not visited: visited = {}
        visited[key] = None
        if func(key): visited[key] = depth
        elif depth < max_depth:
            for edge in self.edges[key]:
                if not edge in visited:
                    visited.update(self.deepsearch(edge, func, depth+1, visited))
        return visited
    
    def shallowsearch(self, keys, func, depth=0, visited=None, max_depth=float('inf')):
        if not visited: visited = {}
        for key in keys:
            if not key in visited: visited[key] = None
            if func(key):
                visited[key] = depth
                return visited
        if depth < max_depth:
            keys = set([k for key in keys for k in self.edges[key]])
            visited.update(self.shallowsearch(keys, func, depth+1, visited))
        return visited
            
    """
        INFO - 
        dijsktra modified from:
        https://gist.github.com/econchick/4666413
    """
    def dijsktra(self, initial, goal=None, start_idx=0):

        visited = {initial: 0}
        path = {}
        nodes = set(self.nodes)

        while nodes:
            min_node = None
            for node in nodes:
                if node in visited:
                    if min_node is None:
                        min_node = node
                    elif visited[node] < visited[min_node]:
                        min_node = node
            if min_node is None:
                break
            nodes.remove(min_node)
            current_weight = visited[min_node]

            gen = self.get_step(min_node, initial, path)

            for edge in self.edges[min_node]:
                distance = self.get_distance((min_node, edge), gen+start_idx)
                weight = current_weight + distance
                if edge not in visited or weight < visited[edge]:
                    
                    visited[edge] = weight
                    path[edge] = min_node
                    
                if goal != None and edge == goal: return visited, path
                        
        return visited, path

    def shortest_path(self, initial_node, goal_node, start_idx=0, paths=None):
        
        if paths == None:
            distances, paths = self.dijsktra(initial_node, goal_node, start_idx)
            
        route = [goal_node]
        while goal_node != initial_node:
            route.append(paths[goal_node])
            goal_node = paths[goal_node]
        route.reverse()
        return route

class Map(Graph):
    
    def __init__(self, floor):
        super(Map, self).__init__()
        self._floor = floor
        self._barrier_map = defaultdict(float)
        self._density_map = defaultdict(lambda : defaultdict(int))
        self._occupancy_map = defaultdict(lambda : defaultdict(int))
        
    @property
    def barrier_map(self):
        return self._barrier_map
    
    def set_barrier(self, key, val):
        self.barrier_map[key] += val
   
    def get_barrier_map(self):
        return [self.barrier_map[k] for k in self.barrier_map.keys()]
        
    @property
    def density_map(self):
        return self._density_map
    
    def get_density(self, key, gen):
        #return self.density_map[gen][key] / self.floor.grid_size**2
        return self.density_map[gen][key]
    
    def get_density_map_at(self, gen):
        return [self.get_density(k, gen) for k in range(len(self.floor.grid))]
    
    def add_density(self, key, gen):
        self.density_map[gen][key] += 1
        
    @property
    def occupancy_map(self):
        return self._occupancy_map
    
    def get_occupancy(self, key, gen):
        return self.occupancy_map[gen][key]
    
    def get_occupancy_map_at(self, gen):
        return [self.get_occupancy(k, gen) for k in range(len(self.floor.grid))]
    
    def add_occupancy(self, key, gen):
        self.occupancy_map[gen][key] += 1
        
    @property
    def floor(self):
        return self._floor
    
    def filter(self, key):
        return self.barrier_map[key]
    
    def get_step(self, initial, goal, route):
        path = [initial]
        while initial != goal:
            path.append(route[initial])
            initial = route[initial]
        curr_gen = len(path)
        return curr_gen
    
    def get_distance(self, key, gen):
        dist = self.distances[key]
        density  = self.get_density(key[1], gen)
        overlays = self.filter(key[1])
        return dist + overlays + density

class Bounds2d():
    
    def __init__(self, **kwargs):
                
        if 'geo' in kwargs:
            self.geo_pts = rs.CurvePoints(kwargs['geo'])

            self._bds = [self.bds_x(self.geo_pts), self.bds_y(self.geo_pts)]
            self._origin = rc.Geometry.Point3d(self.bds[0][0], self.bds[1][0], 0)
            
            self._dim_x = self._bds[0][1] - self._bds[0][0]
            self._dim_y = self._bds[1][1] - self._bds[1][0]
            
        elif 'pts' and 'dim_x' and 'dim_y' in kwargs:
            self.geo_pts = kwargs['pts']
            
            self._bds = [self.bds_x(self.geo_pts), self.bds_y(self.geo_pts)]
            self._origin = rc.Geometry.Point3d(self.bds[0][0], self.bds[1][0], 0)
            
            self.dim_x = kwargs['dim_x']
            self.dim_y = kwargs['dim_y']
            
    @property
    def pts(self):
        return [ rc.Geometry.Point3d(self.bds[0][0], self.bds[1][0], 0),
            rc.Geometry.Point3d(self.bds[0][1], self.bds[1][0], 0),
            rc.Geometry.Point3d(self.bds[0][1], self.bds[1][1], 0),
            rc.Geometry.Point3d(self.bds[0][0], self.bds[1][1], 0)]
    
    @property
    def bds(self):
        return self._bds
    
    @property
    def origin(self):
        return self._origin
    
    def set_origin(self, pt):
        self._origin = pt
    
    @property
    def plane(self):
        return rc.Geometry.Plane(self.origin, rc.Geometry.Vector3d(0,0,1))
    
    @property
    def x(self):
        return self.bds[0][0]
    
    @property
    def y(self):
        return self.bds[1][0]
    
    def bds_x(self, pts):
        pts.sort(key = lambda pt: pt.X)
        return (pts[0].X, pts[-1].X)
    
    def bds_y(self, pts):
        pts.sort(key = lambda pt: pt.Y)
        return (pts[0].Y, pts[-1].Y)
   
    @property
    def dim_x(self):
        return self._dim_x
    
    @property
    def dim_y(self):
        return self._dim_y
    
    @property
    def centroid(self):
        x_average = self.x + self.dim_x/2
        y_average = self.y + self.dim_y/2
        return rc.Geometry.Point3d(x_average, y_average, 0)
    
    def to_rect(self):
        return rc.Geometry.Rectangle3d(self.plane, self.dim_x, self.dim_y)
    
    def to_grid(self, grid_size):
        
        def mesh_from_rect(rect, grid_size):

            mesh_params = rc.Geometry.MeshingParameters.Default
            mesh_params.MaximumEdgeLength = grid_size
            mesh_params.MinimumEdgeLength = grid_size
            mesh_params.GridAspectRatio = 1

            rect = rc.Geometry.Rectangle3d.ToNurbsCurve(rect)
            return rc.Geometry.Mesh.CreateFromPlanarBoundary(rect, mesh_params)
        
        return mesh_from_rect(self.to_rect(), grid_size)
    
    def contains_pt(self, pt):
        pt = rs.coerce3dpoint(pt)
        if pt.X < self.x or pt.X > (self.x + self.dim_x) or pt.Y < self.y or pt.Y > (self.y + self.dim_y):
            return False
        return True
    
    def intersects(self, other):
        
        """
            TODO - write an interval class
        """
        if (((self.origin.X < other.origin.X and self.origin.X + self.dim_x > other.origin.X + other.dim_x) or 
            (other.origin.X < self.origin.X and other.origin.X + other.dim_x > self.origin.X + self.dim_x) or
            (self.origin.X < other.origin.X and self.origin.X + self.dim_x > other.origin.X) or
            (other.origin.X < self.origin.X and other.origin.X + other.dim_x > self.origin.X)) and 
            ((self.origin.Y < other.origin.Y and self.origin.Y + self.dim_y > other.origin.Y + other.dim_y) or 
            (other.origin.Y < self.origin.Y and other.origin.Y + other.dim_y > self.origin.Y + self.dim_y) or
            (self.origin.Y < other.origin.Y and self.origin.Y + self.dim_y > other.origin.Y) or
            (other.origin.Y < self.origin.Y and other.origin.Y + other.dim_y > self.origin.Y))):
                return True
        return False
        
    @classmethod
    def from_center_pt(cls, pt, dim_x, dim_y):
        pt = rs.coerce3dpoint(pt)
            
        geo_pts = [rc.Geometry.Point3d(pt.X+dim_x/2, pt.Y+dim_y/2, pt.Z),
            rc.Geometry.Point3d(pt.X+dim_x/2, pt.Y-dim_y/2, pt.Z),
            rc.Geometry.Point3d(pt.X-dim_x/2, pt.Y-dim_y/2, pt.Z),
            rc.Geometry.Point3d(pt.X-dim_x/2, pt.Y+dim_y/2, pt.Z)]
        
        return cls(pts=geo_pts, dim_x=dim_x, dim_y=dim_y)

class Entity(object):
    
    def __init__(self, attr):
        self._pos = defaultdict(lambda: None)
        self._attr = attr
        self._type = 'entity'
    
    def __repr__(self):
        return "Environment Entity: {0} : {1} ".format(self.type, self.name)

    @property
    def name(self):
        return self.get_attribute('name')

    @property
    def position(self):
        return self._pos[0]
    
    def set_position(self, pos, gen=0):
        self._pos[gen] = pos
    
    def get_position(self, gen=0):
        return self._pos[gen]
    
    @property
    def type(self):
        return self._type
    
    def set_type(self, type):
        self._type = type
    
    @property
    def attributes(self): return self._attr
    
    def set_attribute(self, key, value):
        self._attr[key] = value
    
    def get_attribute(self, key):
        
        def _finditem(obj, key):
            if key in obj: return obj[key]
            for k, v in obj.items():
                if isinstance(v,dict):
                    item = _finditem(v, key)
                    if item is not None:
                        return item
                        
        return _finditem(self.attributes, key)
    
    def has_attribute_value(self, val):
        
        def _findvalue(obj, val):
            if val in obj.values(): return True
            for k,v in obj.items():
                if isinstance(v,dict):
                    value = _findvalue(v, val)
                    if value is not None:
                        return True
            return False
        
        return _findvalue(self.attributes, val)

class Node(Entity):
    
    def __init__(self, pos, attr={}):
        super(Node, self).__init__(attr)
        self.set_position(pos)
        self.set_type('node')

class Template(Entity):
    
    def __init__(self, attr={}):
        super(Template, self).__init__(attr)
        self.set_type('template')
    
    @property
    def edges(self):
        return self.get_attribute('edges')

class Floor(Entity):
    
    def __init__(self, geo, attr={}):
        super(Floor, self).__init__(attr)
        self._geo = rs.coercecurve(geo)
        self._bds = Bounds2d(geo=geo)
        self._map = None
        self._msh = None
        self._grid = None
        self._grid_size = None
        self._coords = {}
        
        self.set_position(self.bounds.origin)
        self.set_type('floor')
        
    @property
    def geo(self):
        return self._geo
    
    @property
    def bounds(self):
        return self._bds
    
    @property
    def mesh(self):
        return self._msh
        
    @property
    def map(self):
        return self._map
    
    @property
    def grid(self):
        return self._grid
        
    def set_grid(self, grid_size):
        
        indexes = []
        denom = m.sqrt(2*(grid_size**2))
        t_mesh = rs.coercemesh(rs.AddPlanarMesh(self.geo))
        self._msh = self.bounds.to_grid(grid_size)
        self._map = Map(self)
        self._grid_size = grid_size
        
        for i, cpt in enumerate(rs.MeshFaceCenters(self.mesh)):
            line = rc.Geometry.Line(cpt, rc.Geometry.Vector3d(0,0,1))
            if not rc.Geometry.Intersect.Intersection.MeshLine(t_mesh, line)[1]:
                indexes.append(i)

        self.mesh.Faces.DeleteFaces(indexes)
        self._grid = rs.MeshFaceCenters(self.mesh)
        
        for i, pt in enumerate(self.grid):
            self.set_coord(self.to_grid_coord(pt), i)
        
        for i, vertex in enumerate(self.mesh.Vertices):

            edges = self.mesh.TopologyVertices.ConnectedFaces(i)
            for e1 in edges:
                for e2 in edges:
                    if not e1 == e2:
                        weight = rs.Distance(self._grid[e1], self._grid[e2])/denom
                        self.map.add_edge(e1,e2, weight)
                        
    @property
    def grid_size(self):
        return self._grid_size
    
    @property
    def coords(self):
        return self._coords
    
    def set_coord(self, key, val):
        self._coords[key] = val
    
    def get_index_at_coord(self, key):
        return self._coords[key]
    
    def to_grid_coord(self, pt):
        
        grid_x = self.bounds.dim_x / m.floor(self.bounds.dim_x / self.grid_size)
        grid_y = self.bounds.dim_y / m.floor(self.bounds.dim_y / self.grid_size)
        
        cs_x = m.floor(self.bounds.dim_x / grid_x)
        cs_y = m.floor(self.bounds.dim_y / grid_y)

        pt_x = m.floor((pt.X - self.position.X)/ grid_x)
        pt_y = m.floor((pt.Y - self.position.Y)/ grid_y)
        
        return (pt_x, pt_y)
    
    def to_grid_index(self, pt):
        
        coord = self.to_grid_coord(pt)
        return self.get_index_at_coord(coord)
    
    def to_grid_bounds(self, pt):
        return Bounds2d.from_center_pt(pt, self.grid_size, self.grid_size)
    
    def contains_pt(self, pt):
        if self.bounds.contains_pt(pt):
            if rs.PointInPlanarClosedCurve(pt, self.geo): return True
        return False
    
    def add_edge_map(self):
        
        def _func(key):
            edges = [self.map.barrier_map[k] for k in self.map.edges[key]]
            if len(edges) != 8 or float('inf') in edges:
                return True
            return False
        
        """
            INFO - parallel toggle
        """
        """
        for i, pt in enumerate(self.grid):
            _vals = [v for v in self.map.shallowsearch([i], _func).values() if not v==None]
            _vals.sort()
            if _vals:
                _weight = 1 / (self.grid_size*(_vals[0]+1))
                self.map.set_barrier(i, _weight)
        """
        
        def _parallelsearch(i):
            if not self.map.barrier_map[i] == float('inf'):
                _vals = [v for v in self.map.shallowsearch([i], _func).values() if not v==None]
                _vals.sort()
                if _vals:
                    _weight = 1 / (self.grid_size*(_vals[0]+1))
                    self.map.set_barrier(i, _weight)
        
        ghpythonlib.parallel.run(_parallelsearch, range(len(self.grid)), False)
        
                
    def add_barrier_map(self, barrier):
        
        for i, pt in enumerate(self.grid):
            if barrier.bounds.contains_pt(pt):
                if rs.PointInPlanarClosedCurve(pt, barrier.geo):
                    self.map.set_barrier(i, float('inf'))
            
            else:
                bds = self.to_grid_bounds(pt)
                if barrier.bounds.intersects(bds):
                    for pt in bds.geo_pts:
                        """
                            TODO - maybe this can be done more effectively
                        """
                        if rs.PointInPlanarClosedCurve(pt, barrier.geo):
                            self.map.set_barrier(i, float('inf'))
                            break
            
class Barrier(Entity):
    
    def __init__(self, geo, attr={}):
        super(Barrier, self).__init__(attr)
        self._geo = rs.coercecurve(geo)
        self._bds = Bounds2d(geo=geo)
        
        self.set_position(self.bounds.origin)
        self.set_type('barrier')
        
    @property
    def geo(self):
        return self._geo
    
    @property
    def bounds(self):
        return self._bds
    
    @property
    def floor(self):
        return self.get_attribute('floor')
        
class Agent(Entity):
    
    def __init__(self, pos=None, attr={}, env=None):
        super(Agent, self).__init__(attr)
        self._attr = attr
        self._env = env
        self._state = None
        self._history = []
        self._path = []
        
        self.set_position(pos)
        self.set_type('agent')
        
        self.age = 0
        self.__hold = None
        self.is_active = True
        self.is_complete = False
                
    @property
    def position(self):
        return self.get_position(self.age)
        
    def shift(self):
        
        """
            TODO - not clean - Make this nicer
        """
        next_position = self.state[-2]
        if type(next_position) == Node:
            next_position = self.environment.floors[0].to_grid_index(next_position.position)
            
        _index =  self._path[-1].index(next_position)
        self._path[-1].insert(_index, next_position)
        self.history.append(self.position)
        
        self.environment.floors[0].map.add_occupancy(self.position, self.age)

        keys = self._pos.keys()
        keys.sort()
        keys = keys[self.age:]
        keys.reverse()
        _dict = copy.copy(self._pos)
        for ki in range(len(keys)):
            
            curr_gen = keys[ki]
            curr_pos = _dict[curr_gen]
            
            next_gen = keys[ki]+1
            next_pos = _dict[next_gen]
            
            self.environment.floors[0].map.density_map[next_gen][next_pos] -=1
            self.environment.floors[0].map.density_map[next_gen][curr_pos] +=1
            
            _dict[next_gen] = _dict[curr_gen]
        
        self._pos = _dict
        
    @property
    def grid_index(self):
        return self.environment.floors[0].to_grid_index(self.position.position)
    
    @property
    def state(self):
        return self._state
    
    def set_state(self, state):
        self._state = state
    
    @property
    def destination(self):
        val = self.get_attribute('destination')
        return random.choice(self.environment.get_nodes_with_value(val))
    
    @property
    def path(self):
        _path = sum([p for p in self._path], [])
        _dict = {idx: self.environment.floors[0].grid[idx] for idx in set(_path)}
        return [_dict[p] for p in _path]
    
    def add_path(self, path):
        self._path.append(copy.deepcopy(path))    
    
    @property
    def history(self):
        return self._history
    
    @property
    def visited(self):
        return [n.name for n in self.history if not type(n) == int]
    
    @property
    def environment(self):
        return self._env
    
    def set_environment(self, env):
        self._env = env
        
    def toggle_active(self):
        self.is_active = not self.is_active
        
    def toggle_complete(self):
        self.is_complete = not self.is_complete
    
    def step(self):        
        
        def wait(time):
            return self.position, [self.position] * time
            
        def move(lst):
            
            lst = [l for l in lst if not l.get_attribute('name') in self.visited]
            
            if len(lst):
            
                properties = self.get_attribute('properties')
                keys = properties.keys()
                n_percs = [k for k in keys if random.randint(0,100) <= float(properties[k])]
                
                lst = [(l, self.environment.graph.get_distance((self.position, l))) for l in lst if l.get_attribute('name') in n_percs]
            
                if len(lst):
                    
                    lst.sort(key = lambda tup: len(tup[1]))
                    lst.reverse()
                    
                    while len(lst):
                        
                        tup = lst.pop()
                        n_choice = tup[0]
                        
                        num_people = len([a for a in self.environment.agents if self.__hold == a.__hold and a != self])
                        capacity = n_choice.get_attribute('capacity')

                        if capacity != None:
                            if num_people < float(capacity):
                                break
                            else:
                                if random.randint(0,100) < self.get_attribute('queuing'):
                                    break
                                else: 
                                    continue
                        else:
                            break
                else:
                    n_choice = self.destination
            else:
                n_choice = self.destination
                self.toggle_complete()
            
            """
                TODO - replace with entity.to_grid_index(environment)
                    nodes need floor property !! important
            """
            n_index = self.environment.floors[0].to_grid_index(n_choice.position)
            n_path = self.environment.floors[0].map.shortest_path(self.grid_index, n_index, start_idx=self.age)

            return n_choice, n_path

        if not len(self.history):

            start_node = self.get_attribute('origin')
            start_positions = self.environment.get_nodes_with_value(start_node)
            start_position = random.choice(start_positions)
            
            self.set_position(start_position)
            self.set_state('waiting')
            
        else:

            if self.is_active:
                
                if self.state == 'ready':
                    
                    self.__hold, path = move(self.environment.graph.edges[self.position])
                    self.add_path(path[:-1])
                    
                    for pi in range(len(path)):
                        if pi != len(path)-1:
                            self.environment.floors[0].map.add_density(path[pi], self.age+pi)
                        self.set_position(path[pi], self.age+pi)
                    
                    path.reverse()
                    self.set_state(path)
               
               
                if self.state == 'waiting':
                    
                    num_people = len(self.environment.agents_at_position(self.__hold, self.age))+1
                    """
                        TODO - switch to property of node/agent?
                        this is also where the intial arrival time would be implemented and swapped
                    """
                    wait_time = num_people * 1 if len(self._path) != 0 else num_people * random.randint(1,60)
                                        
                    self.__hold, path = wait(wait_time)
                    self.add_path([self.grid_index] * (wait_time-1))
                    
                    for pi in range(len(path)):
                        if pi != len(path)-1:
                            self.environment.floors[0].map.add_density(self.grid_index, self.age+pi)
                        self.set_position(path[pi], self.age+pi)
                    
                    path.reverse()
                    self.set_state(path)
                    
                elif len(self.state) == 1:

                    next_position = self.__hold
                    capacity = next_position.get_attribute('capacity')
                    capacity = int(capacity) if capacity != None else float('inf')
                    num_people = len(self.environment.agents_at_position(next_position, self.age))

                    if num_people > capacity and type(self.position)!= Node:
                        self.state.append(self.position)
                        self.shift()
                        
                        self.age+=1
                        self.state.pop();
                    
                    else:
                        self.set_position(self.__hold, self.age)
                        
                        if self.is_complete:
                            self.set_state('complete')
                            self.toggle_active()
                        else:
                            if self.position == self.state[0]: self.set_state('ready')
                            else: self.set_state('waiting')
                            
                else:

                    next_position = self.state[-2]
                    max_density = 1 * self.environment.resolution 
                    num_people = len(self.environment.agents_at_position(next_position, self.age))
                    next_people = len(self.environment.agents_at_position(next_position, self.age+1))
                    """
                        TODO - handle infinite while loop, 
                            this is a temporary solution
                    """
                    """
                    curr_people = len(self.environment.agents_at_position(self.position, self.age))
                    
                    if curr_people > max_density and next_people > max_density:
                        if type(self.position)!= Node:
                            print 'critical density met', next_position
                            max_density += num_people
                    """
                    if num_people > max_density or next_people > max_density:
                        if type(self.position)!= Node:
                            self.state.append(self.position)
                            self.shift()
                        
                    self.age+=1
                    self.state.pop()
                    
        self.history.append(self.position)
        
class Environment():
    
    def __init__(self, resolution=30):
        self._entities = defaultdict(list)
        self._agents = []
        self._graph = Graph()
        self._graph_paths = {}
        self._resolution = resolution
    
    def __repr__(self):
        return 'Environment: {0} agents, {1} nodes, {2} floors'.format(len(self.agents), len(self.graph.nodes), len(self.floors))
    
    @property
    def resolution(self):
        return self._resolution
    
    @property
    def entities(self):
        return self._entities
     
    def add_entity(self, entity):
        self.entities[entity.type].append(entity)
    
    def add_entities(self, entities):
        
        def _add_floor(entity):
            entity = Floor(entity['geometry'], entity['profile'])
            self.add_floor(entity)
            self.add_entity(entity)
        
        def _add_node(entity):
            entity = Node(entity['position'], entity['profile'])
            _msg = self.add_node(entity)
            if _msg:
                return _msg
            else:
                self.add_entity(entity)
            
        def _add_template(entity):
            entity = Template(entity['profile'])
            self.add_template(entity)
            self.add_entity(entity)
        
        def _add_barrier(entity):
            entity = Barrier(entity['geometry'], entity['profile'])
            self.add_barrier(entity)
            self.add_entity(entity)
        
        def _add_agent(entity):
            entity = Agent(attr=entity['profile'])
            self.add_agent(entity)
            self.add_entity(entity)
        
        dict = defaultdict(list)
        for entity in entities:
            dict[entity['type']].append(entity)
        
        if len(dict['floor']):
            for  floor in dict['floor']:
                _add_floor(floor)
        else:
            e = gh.GH_RuntimeMessageLevel.Error
            ghenv.Component.AddRuntimeMessage(e, 'component needs at least one floor')
            return
            
        if len(dict['node']) > 1:
            """
                INFO - toggle parallel simulation
            """
            """
            for node in dict['node']:
                _add_node(node)
            """
            _err = ghpythonlib.parallel.run(_add_node, dict['node'], False)
            if any( _err):
                for _msg in _err:
                    e = e = gh.GH_RuntimeMessageLevel.Error
                    ghenv.Component.AddRuntimeMessage(e, _msg)
                return

        else:
            e = gh.GH_RuntimeMessageLevel.Error
            ghenv.Component.AddRuntimeMessage(e, 'component needs at least two nodes')
            return
              
        if len(dict['template']):
            for template in dict['template']:
                _add_template(template)
        else: self.add_default_template()

        if len(dict['barrier']):
            for barrier in dict['barrier']:
                _add_barrier(barrier)
                
        for floor in self.entities['floor']:
            floor.add_edge_map()
            
        if len(dict['agent']):
            for agent in dict['agent']:
                _add_agent(agent)
        else:
            w = gh.GH_RuntimeMessageLevel.Warning
            ghenv.Component.AddRuntimeMessage(w, 'no agents in environment')
            
    @property
    def graph(self):
        return self._graph
    
    @property
    def graph_paths(self):
        return self._graph_paths
    
    def get_graph_path(self, n1, n2):
        """
            TODO - if n1 and n2 are on different floors -> handle graph links
                - nodes need a floor attribute
        """
        floor = self.floors[0]
        idx1 = floor.to_grid_index(n1.position)
        idx2 = floor.to_grid_index(n2.position)
        return floor.map.shortest_path(idx1, idx2, paths=self.graph_paths[n1])
    
    def set_graph_path(self, key, val):
        self._graph_paths[key] = val
    
    @property
    def templates(self):
        return [n for n in self.entities['template']]
    
    def add_template(self, template):
        for edge in template.edges:
            nodes = [n for n in self.nodes if n.position in edge]
            if len(nodes) == 2:
                try:
                    path = self.get_graph_path(nodes[0], nodes[1])
                except:
                    path = self.get_graph_path(nodes[1], nodes[0])
                    
                self.graph.add_edge(nodes[0], nodes[1], path,
                    directed=template.get_attribute('directed'))
    
    def add_default_template(self):
        for n1 in self.graph.nodes:
            for n2 in self.graph.nodes:
                if not n1 == n2: 
                    path = self.get_graph_path(n1, n2)
                    self.graph.add_edge(n1, n2, path)
                    
    @property
    def agents(self):
        return self._agents
        
    def add_agent(self, agent, other=None):
        agent.set_environment(self)
        self.agents.append(agent)
    
    def agents_at_position(self, position, gen=0):
        return [a for a in self.agents if a.get_position(gen) == position]
        
    @property
    def nodes(self):
        return [n for n in self.entities['node']]
    
    def add_node(self, node):
        self.graph.add_node(node)
        """
            TODO - nodes need a floor attribute
        """
        if self.floors[0].contains_pt(node.position):
            djk = self.floors[0].map.dijsktra(self.floors[0].to_grid_index(node.position))
            self.set_graph_path(node, djk[1])
        else:
            return node.name +' was flagged as not on a floor'

    def get_nodes_with_value(self, val):
        return [n for n in self.nodes if n.has_attribute_value(val)]
        
    @property
    def floors(self):
        return [n for n in self.entities['floor']]
    
    def add_floor(self, floor):
        floor.set_grid(self.resolution)
    
    def set_grid(self, grid_size):
        """
            TODO - implement more complexity
            grids will need to match up between floors
               - maybe 1 unified graph to hold graphs?
        """
        for floor in self.floors: floor.set_grid(grid_size)
        
    @property
    def barriers(self):
        return [n for n in self.entities['barriers']]
    
    def add_barrier(self, barrier):
        for floor in self.floors:
            if barrier.floor == floor.name:
                floor.add_barrier_map(barrier)
                
    @property
    def paths(self):
        return [a.path for a in self.agents]
        
    def generation(self, gen):
        return [a.get_position(gen) for a in self.agents]

    def step(self):
        
        def _func(agent):
            if agent.is_active:
                agent.step()
                   
        """
            INFO - toggle parallel simulation
        """
        """
        for i, agent in enumerate(self.agents):
            _func(agent)
        """
        ghpythonlib.parallel.run(_func, self.agents, False)
            
    def run(self):
        count = 0
        while len([a for a in self.agents if a.is_active]) and count < 1000:
            self.step()
            count+=1
            
        if count == 1000: print ">>>>>>>>>>>>>>>>>>>>>>>>> broke outer while loop"
        

if len(entities) != 0 and res != None:
    environment = Environment(resolution=res)
    environment.add_entities([e for e in entities if e != None])
    
    a = environment
