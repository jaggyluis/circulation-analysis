import rhinoscriptsyntax as rs
import Rhino as rc
    
if len(floor) != 0 and boundary != None:
    
    entities = []
    boundary.reverse()
    
    for curve in boundary:
        
        crv = rs.coercecurve(curve)

        if crv.IsClosed:
            
            entity = {
                'geometry':crv,
                'type':'barrier',
                'profile':{
                    'name':None,
                    'floor':floor[-1]
                }
            }
            if len(floor) > 1: floor.pop()
            entities.append(entity)
        
        else: print 'curve not closed... ignoring'
    
    for e in entities: print e
    a = entities
