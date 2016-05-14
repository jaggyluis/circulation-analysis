
/*
 * Graph Object
 */
var Graph = function() {
        this.nodes = new Set();
        this.distances = {};
        this.distances.set = function(arr, val) {
        	var key = JSON.stringify(arr);
        	this[key] = val;
        }
        this.distances.get = function(arr) {
        	var key = JSON.stringify(arr);
        	return this[key];
        }
        this.edges = {};
};
Graph.prototype.addNode = function(node) {
	this.nodes.add(node);
	if (Object.keys(this.edges).indexOf(node) === -1) {
		this.edges[node] = [];
	}
};
Graph.prototype.addEdge = function(fromNode, toNode, distance) {
        this.addNode(fromNode);
        this.addNode(toNode);

        this.edges[fromNode].push(toNode);
        this.edges[toNode].push(fromNode);

        this.distances.set([toNode, fromNode], distance);
        this.distances.set([fromNode, toNode], distance);
};
Graph.prototype.dijsktra = function(initialNode) {

        var visited = {};
        var path = {};
        var nodes = new Set(this.nodes);

        visited[initialNode] = 0;

        while (nodes) {
        	var minNode = false;

        	nodes.forEach(function(node) {
        		
        		if (Object.keys(visited).indexOf(node) !== -1) {
        			if (!minNode) {
        				minNode = node;
        			}
        			else if (visited[node] < visited[minNode]) {
        				minNode = node;
        			}
        		}
        	});
        	if (minNode === false) {
        		break;
        	}
        	nodes.delete(minNode);

        	var currentWeight = visited[minNode];
        	this.edges[minNode].forEach((function(edge) {

        		var weight = currentWeight + this.distances.get([minNode, edge]);

        		if (!(edge in visited) || weight < visited[edge]) {
        			visited[edge] = weight;
        			path[edge] = minNode;
        		}
        	}).bind(this));
        }
        return {visited:visited, path:path};

};
Graph.prototype.shortestPath = function(initialNode, goalNode) {

	var dijsktra = this.dijsktra(initialNode);
	var distances = dijsktra.visited,
		paths = dijsktra.path;
        var route = [goalNode];

        while (goalNode !==  initialNode) {
        	route.push(paths[goalNode]);
        	goalNode = paths[goalNode];
        }
        route.reverse();
        return route;
};


/*
 * Vector Object
 */
var Vec = function(x,y) {
	this.x = x;
	this.y = y
};
Vec.prototype.plus= function(other) {
	return new Vec(this.x + other.x, this.y + other.y);
};
Vec.prototype.distance = function(other) {
	return Math.sqrt(Math.pow(this.x-other.x, 2) + Math.pow(this.y-other.y, 2));
};


/*
 * Node Object - Will become Point
 */
var Node = function(x, y) {
	this.pos = new Vec(x,y);
};
Object.defineProperties(Node, {
	'x': {get: function() {return this.pos.x}},
	'y': {get: function() {return this.pos.y}}
});
Node.prototype.draw = function(ctx, color, sizeX, sizeY) {
    ctx.fillStyle = color ;
	ctx.fillRect(this.pos.x*10, this.pos.y*10, sizeX? sizeX: 1, sizeY? sizeY :1);
};
Node.prototype.move = function(vec) {
	this.pos = this.pos.plus(vec);
};
Node.prototype.valueOf = function valueOf() {
	return JSON.stringify(this.pos);
};
Node.prototype.distance = function(other) {
	return this.pos.distance(other.pos);
};


/*
 * Grid Object
 */
var Grid  = function(dimX, dimY, item) {
        this.rows = [];

        for (var i=0; i<dimX; i++) {
            var row = [];
            for (var j=0; j < dimY; j++) {
                row.push(new item(i,j));
            }
            this.rows.push(row);
        }
};
Grid.prototype.getNodesArray = function() {
    return this.rows.reduce(function(a,b) {
        return a.concat(b);        
    },[]);
};
Grid.prototype.getNodesObject = function() {
    return this.getNodesArray().reduce(function(a,b) {
                a[b.valueOf()] = b;
                return a;
        }, {});
};
Grid.prototype.applyGraph = function(edgeMethod, graph) {

    if (graph === undefined) var graph = new Graph();

    for (var x=0; x<this.rows.length; x++) {
        for (var y = 0; y < this.rows[x].length; y++) {
            for (var i=-1 ; i<2; i++) {
                for (var j=-1 ; j<2; j++) {
                    var q = x+i,
                        r = y+j;

                    if(!(q===x && r===y) && 
                        (q >=0 && r >=0) && 
                        (q < this.rows.length && r < this.rows[x].length)) {
                        if (this.rows[q][r] !== undefined) {
                            graph.addEdge(this.rows[x][y].valueOf(),
                                this.rows[q][r].valueOf(),
                                edgeMethod(this.rows[x][y], this.rows[q][r]));
                                //this.rows[x][y].distance(grid[q][r]));
                        }
                    }

                }
            }

        }
    }
    return graph;
};