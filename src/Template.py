import rhinoscriptsyntax as rs
import Rhino as rc


if len(lines):

    entities = []
    for line in lines:
        entities.append([rs.CurveEndPoint(line), rs.CurveStartPoint(line)])
    entity = {
        'type': 'template',
        'profile': {
            'edges': entities,
            'directed': directed if directed else False
        }
    }
    print entity
    a = [entity]
