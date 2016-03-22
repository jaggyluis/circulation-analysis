
import rhinoscriptsyntax as rs
    
if len(name) != 0 and boundary != None:
    
    entities = []
    boundary.reverse()
    name.reverse()
    
    for curve in boundary:
        
        crv = rs.coercecurve(curve)
        if crv.IsClosed:
        
            entity = {
                'geometry':rs.coercecurve(curve),
                'type':'floor',
                'profile':{
                    'name': name[-1]
                }
            }
            if len(name) > 1: name.pop()
            entities.append(entity)
        
        else: print 'curve not closed... ignoring'
    
    for e in entities: print e
    a = entities