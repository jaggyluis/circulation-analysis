import rhinoscriptsyntax as rs
import copy, random

           
if name != None and origin != None and destination != None:

    nodes.reverse()
    vals.reverse()

    entity = {
        'type': 'agent',
        'profile': {
            'name': name,
            'origin': origin,
            'destination': destination,
            'properties':{
            }
        },
        'influence':{
            'queuing':50, #implement later
            'remainder':None # implement later
        }
    }
    
    if len(nodes) > 0 and len(vals) > 0:
        count=0
        while len(nodes) and count < 100:
            count+=1
            entity['profile']['properties'][nodes.pop()] = vals.pop() if len(vals) > 1 else vals[-1]
    
    
    entity['profile']['properties'][origin] = 100
    entity['profile']['properties'][destination] = 100
    
    print entity
    a = [entity]