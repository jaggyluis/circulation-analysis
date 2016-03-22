
    barrier_map = env.floors[0].map.get_barrier_map()

    if run:
    
        env = copy.deepcopy(env)
        env.run()
        
        for i, path in enumerate(env.paths):
            gh_path = GH_Path(i)
            for p in path: path_tree.Add(p, gh_path)
        
        
        activity = [0] * len(env.floors[0].grid)
        occupancy = [0] * len(env.floors[0].grid)
    
        for gen in env.floors[0].map.density_map.keys():
            
            gh_path = GH_Path(gen)
            
            d_map = env.floors[0].map.get_density_map_at(gen)
            for i, m in enumerate(d_map):
                activity[i] += m
                density_tree.Add(m, gh_path)
                
            o_map = env.floors[0].map.get_occupancy_map_at(gen)
            for j, n in enumerate(o_map):
                occupancy[j] += n
                
        
        activity_map = activity
        occupancy_map = occupancy
        density_map = density_tree
        paths = path_tree
