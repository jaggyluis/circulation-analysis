import copy
import rhinoscriptsyntax as rs

if len(name) != 0 and position!= None:
    
    entities = []
    capacity = [None] if (len(capacity) == 0 or capacity == 0) else capacity
    
    position.reverse()
    name.reverse()
    capacity.reverse()
    
    for p in position:
        
        entity = {
            'position':rs.coerce3dpoint(p),
            'type': 'node',
            'profile': {
                'name': name[-1],
                'capacity': capacity[-1]
            }
        }
        if len(name) > 1: name.pop()
        if len(capacity) > 1: capacity.pop()
        entities.append(entity)

    for e in entities: print e
    a = entities