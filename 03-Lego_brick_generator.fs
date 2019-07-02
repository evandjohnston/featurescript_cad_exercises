FeatureScript 1096;
import(path : "onshape/std/geometry.fs", version : "1096.0");

export const mm = millimeter as ValueWithUnits;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* fCuboidBool creates a cuboid at the given coordinates,
* and performs a given boolean operation with a given
* existing body.
*
* It takes arguments of context, id, and a map which
* specifies two corners of the cuboid, a body to 
* boolean with, and the boolean operation to perform.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function fCuboidBool(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Coordinates of Corner 1" }
    is3dLengthVector(definition.corner1);
    
    annotation { "Name" : "Coordinates of Corner 2" }
    is3dLengthVector(definition.corner2);
    
    annotation { "Name" : "Body to Add to", "Filter" : EntityType.BODY, "MaxNumberOfPicks" : 1 }
    definition.targets is Query;
    
    annotation { "Name" : "Add or Subtract cuboid" }
    definition.opType is BooleanOperationType;
}
{
    fCuboid(context, id + "cuboid1", {
            "corner1" : definition.corner1,
            "corner2" : definition.corner2,
    });
    
    opBoolean(context, id + "fCuboidBool", {
        "tools" : qCreatedBy(id + "cuboid1", EntityType.BODY),
        "targets" : definition.targets,
        "targetsAndToolsNeedGrouping" : true,
        "operationType" : definition.opType,
    });
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* fCylinderBool creates a cylinder of a given radius at the 
* given coordinates, and performs a given boolean 
* operation with a given existing body.
*
* It takes arguments of context, id, and a map which
* specifies centers of the top and bottom faces of the
* cylinder, a body to boolean with, and the boolean 
* operation to perform.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function fCylinderBool(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Top Center" }
    is3dLengthVector(definition.topCenter);
    
    annotation { "Name" : "Bottom Center" }
    is3dLengthVector(definition.botCenter);
    
    annotation { "Name" : "Radius" }
    isLength(definition.radius);
    
    annotation { "Name" : "Body to Add to", "Filter" : EntityType.BODY, "MaxNumberOfPicks" : 1 }
    definition.targets is Query;
    
    annotation { "Name" : "Add or Subtract Cylinder" }
    definition.opType is BooleanOperationType;
}
{
    fCylinder(context, id + "cylinder1", {
        "topCenter" : definition.topCenter,
        "bottomCenter" : definition.botCenter,
        "radius" : definition.radius,
    });
    
    opBoolean(context, id + "fCylinderBool", {
        "tools" : qCreatedBy(id + "cylinder1", EntityType.BODY),
        "targets" : definition.targets,
        "targetsAndToolsNeedGrouping" : true,
        "operationType" : definition.opType,
    });
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* fText creates a 3D text string which fits into a given 
* area on a given plane, and extrudes it a given distance
* normal to the surface of the plane.
*
* It takes arguments of context, id, and a map which
* specifies the plane to place the text on, the area in
* which to fit the text, and the distance to extrude the
* text from the plane.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function fText(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Text" }
    definition.text is string;
    
    annotation { "Name" : "Sketch Plane" }
    definition.skPlane is Plane;
    
    annotation { "Name" : "First Corner" }
    is2dPointVector([definition.firstCorner]);
    
    annotation { "Name" : "Second Corner" }
    is2dPointVector([definition.secondCorner]);
    
    annotation { "Name" : "Extrude Distance" }
    isLength(definition.extDistance);
}
{
    var textSketch is Sketch = newSketchOnPlane(context, id + "studLetterSketch", {
        "sketchPlane" : definition.skPlane,
    });
    
        skText(textSketch, "text1", {
            "text" : definition.text,
            "fontName" : "OpenSans-Regular.ttf",
            "firstCorner" : definition.firstCorner,
            "secondCorner" : definition.secondCorner
        });
        
    skSolve(textSketch);
    
    var textToExtrude = qSketchRegion(id + "studLetterSketch", true);
    opExtrude(context, id + "extrudeText", {
        "entities" : textToExtrude,
        "direction" : definition.skPlane.normal,
        "endBound" : BoundingType.BLIND,
        "endDepth" : definition.extDistance,
    });
    
    opDeleteBodies(context, id + "deleteBodies1", {
        "entities" : qSketchFilter(qCreatedBy(id), SketchObject.YES)
    });
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* fTextBool creates a 3D text string which fits into a 
* given area on a given plane, extrudes it a given distance
* normal to the surface of the plane, and performs a given 
* boolean operation with a given existing body.
*
* It takes arguments of context, id, and a map which
* specifies the plane to place the text on, the area in
* which to fit the text, the distance to extrude the
* text from the plane, a body to boolean with, and the 
* boolean operation to perform.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function fTextBool(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Text" }
    definition.text is string;
    
    annotation { "Name" : "Sketch Plane" }
    definition.skPlane is Plane;
    
    annotation { "Name" : "First Corner" }
    is2dPointVector([definition.firstCorner]);
    
    annotation { "Name" : "Second Corner" }
    is2dPointVector([definition.secondCorner]);
    
    annotation { "Name" : "Extrude Distance" }
    isLength(definition.extDistance);
    
    annotation { "Name" : "Body to Add to", "Filter" : EntityType.BODY, "MaxNumberOfPicks" : 1 }
    definition.targets is Query;
    
    annotation { "Name" : "Add or Subtract Text" }
    definition.opType is BooleanOperationType;
}
{
    fText(context, id + ("textBool"), {
          "text" : definition.text,
          "skPlane" : definition.skPlane,
          "firstCorner" : definition.firstCorner,
          "secondCorner" : definition.secondCorner,
          "extDistance" : definition.extDistance,
    });
    
    opBoolean(context, id + "fTextBool", {
        "tools" : qCreatedBy(id + "textBool", EntityType.BODY),
        "targets" : definition.targets,
        "targetsAndToolsNeedGrouping" : true,
        "operationType" : definition.opType,
    });
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* fLego creates a Lego brick of given dimensions in brick
* studs at a given location in 3D space, with a given
* text string embossed on the studs.
* 
* It takes arguments of context, ID, and a map which
* specifies the brick's dimensions in brick studs, its
* location, and the text to emboss on the studs.
* 4-character uppercase text strings work best.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function fLego(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Brick Origin" }
    is3dLengthVector(definition.origin);
    
    annotation { "Name" : "Stud Dimensions" }
    isUnitlessVector(definition.dimensions);
    
    annotation { "Name" : "Embossed Text" }
    definition.text is string;
}
{
    var brickOrigin     is Vector = definition.origin;
    var brandText       is string = definition.text;

    var gridSize        is ValueWithUnits = 8 * mm;
    var height          is ValueWithUnits = 9.6 * mm;
    var dim             is Vector = definition.dimensions * gridSize;
    var wallThickness   is ValueWithUnits = 1.6 * mm;
    var pipeThickness   is ValueWithUnits = wallThickness;
    var supportStrutThk is ValueWithUnits = wallThickness / 2;
    var studThickness   is ValueWithUnits = 1.6 * mm;
    var studRad         is ValueWithUnits = 2.4 * mm;
    var dimpleRad       is ValueWithUnits = 1.6 * mm;
    var postRad         is ValueWithUnits = 1.6 * mm;
    var pipeRad         is ValueWithUnits = 3.2 * mm;
    
    var textWidth       is ValueWithUnits = studRad * 0.95;
    var textHt          is ValueWithUnits = studRad * 0.55;
    var textThickness   is ValueWithUnits = 0.05 * mm;
    
    var supportStrutHt  is ValueWithUnits = height - (studThickness + 2 * textThickness);
    
    //Create box
    fCuboid(context, id + "legoBase", {
            "corner1" : brickOrigin,
            "corner2" : brickOrigin + vector(dim[0], dim[1], height),            
    });
    
    fCuboidBool(context, id + "rmBaseInterior", {
        "corner1" : brickOrigin + vector(wallThickness, wallThickness, 0 * mm),
        "corner2" : brickOrigin + vector(dim[0], dim[1], height) - vector(wallThickness, wallThickness, wallThickness),
        "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
        "opType" : BooleanOperationType.SUBTRACTION,
    });
    

    for (var x = 0; x < definition.dimensions[0]; x += 1)
    {
        for (var y = 0; y < definition.dimensions[1]; y += 1)
        {
            // Add studs and text, remove dimples
            fCylinderBool(context, id + ("stud"~x~y), {
                "topCenter" : brickOrigin + vector(x * gridSize + (gridSize / 2), y * gridSize + (gridSize / 2), height + studThickness),
                "botCenter" : brickOrigin + vector(x * gridSize + (gridSize / 2), y * gridSize + (gridSize / 2), height),
                "radius" : studRad,
                "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                "opType" : BooleanOperationType.UNION,
            });
            
            fCylinderBool(context, id + ("dimple"~x~y), {
                "topCenter" : brickOrigin + vector(x * gridSize + (gridSize / 2), y * gridSize + (gridSize / 2), height),
                "botCenter" : brickOrigin + vector(x * gridSize + (gridSize / 2), y * gridSize + (gridSize / 2), height - wallThickness),
                "radius" : dimpleRad,
                "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                "opType" : BooleanOperationType.SUBTRACTION,
            });
            
            fTextBool(context, id + ("text"~x~y), {
                  "text" : brandText,
                  "skPlane" : plane(brickOrigin + vector(0 * mm, 0 * mm, height + studThickness), vector(0, 0, 1)),
                  "firstCorner" : vector(((x * gridSize) + gridSize / 2) - (textWidth), ((y * gridSize) + gridSize / 2) - (textHt / 2)),
                  "secondCorner" : vector(((x * gridSize) + gridSize / 2) + (textWidth), ((y * gridSize) + gridSize / 2) + (textHt / 2)),
                  "extDistance" : textThickness,
                  "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                  "opType" : BooleanOperationType.UNION,
            });

            //Create internal supports
            if (definition.dimensions[0] > 1 && x < definition.dimensions[0] - 1 && definition.dimensions[1] > 1 && y < definition.dimensions[1] - 1)
            {
                //Support ribs
                if (x % 2 == 1 || definition.dimensions[0] == 3)
                {
                    fCuboidBool(context, id + ("supportStrutX"~x~y), {
                        "corner1" : brickOrigin + vector((x + 1) * gridSize - supportStrutThk / 2, dim[1], height - supportStrutHt),
                        "corner2" : brickOrigin + vector((x + 1) * gridSize + supportStrutThk / 2, 0 * mm, height),
                        "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                        "opType" : BooleanOperationType.UNION,
                    });
                }
                
                if (y % 2 == 1 || definition.dimensions[1] == 3)
                {
                    fCuboidBool(context, id + ("supportStrutY"~x~y), {
                        "corner1" : brickOrigin + vector(dim[0], (y + 1) * gridSize - supportStrutThk / 2, height - supportStrutHt),
                        "corner2" : brickOrigin + vector(0 * mm, (y + 1) * gridSize + supportStrutThk / 2, height),
                        "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                        "opType" : BooleanOperationType.UNION,
                    });
                }
                
                //Support pipes
                fCylinderBool(context, id + ("pipePositive"~x~y), {
                    "topCenter" : brickOrigin + vector((x + 1) * gridSize, (y + 1) * gridSize, height - wallThickness),
                    "botCenter" : brickOrigin + vector((x + 1) * gridSize, (y + 1) * gridSize, 0 * mm),
                    "radius" : pipeRad,
                    "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                    "opType" : BooleanOperationType.UNION,
                });
            }
            // Support posts
            else if (definition.dimensions[0] == 1 && definition.dimensions[1] > 1 && y < definition.dimensions[1] - 1)
            {
                fCylinderBool(context, id + ("post"~x~y), {
                    "topCenter" : brickOrigin + vector(gridSize / 2, (y + 1) * gridSize, height),
                    "botCenter" : brickOrigin + vector(gridSize / 2, (y + 1) * gridSize, 0 * mm),
                    "radius" : postRad,
                    "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                    "opType" : BooleanOperationType.UNION,
                });
            }
            else if (definition.dimensions[0] > 1 && definition.dimensions[1] == 1 && x < definition.dimensions[0] - 1) {
                fCylinderBool(context, id + ("post"~x~y), {
                    "topCenter" : brickOrigin + vector((x + 1) * gridSize, gridSize / 2, height),
                    "botCenter" : brickOrigin + vector((x + 1) * gridSize, gridSize / 2, 0 * mm),
                    "radius" : postRad,
                    "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                    "opType" : BooleanOperationType.UNION,
                });
            }
        }
    }
    
    if (definition.dimensions[0] > 1 && definition.dimensions[1] > 1)
    {
        for (var x = 0; x < definition.dimensions[0] - 1; x += 1)
        {
            for (var y = 0; y < definition.dimensions[1] - 1; y += 1)
            {
                fCylinderBool(context, id + ("pipeNegative"~x~y), {
                    "topCenter" : brickOrigin + vector((x + 1) * gridSize, (y + 1) * gridSize, height - wallThickness),
                    "botCenter" : brickOrigin + vector((x + 1) * gridSize, (y + 1) * gridSize, 0 * mm),
                    "radius" : pipeRad - pipeThickness,
                    "targets" : qCreatedBy(id + "legoBase", EntityType.BODY),
                    "opType" : BooleanOperationType.SUBTRACTION,
                });
            }
        }
    }
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* createLego creates a Lego brick of given dimensions in 
* brick studs at a given location in 3D space.
* 
* It takes arguments of the brick's width and height in 
* studs, and the minimum X, Y, and Z-coordinates it should
* intersect.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * */
annotation { "Feature Type Name" : "Create Lego" }
export const createLego = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Number of Studs Wide" }
        isInteger(definition.studCountX, POSITIVE_COUNT_BOUNDS);

        annotation { "Name" : "Number of Studs Deep" }
        isInteger(definition.studCountY, POSITIVE_COUNT_BOUNDS);
        
        annotation { "Name" : "X coordinate" }
        isLength(definition.XCoord, LENGTH_BOUNDS);

        annotation { "Name" : "Y coordinate" }
        isLength(definition.YCoord, LENGTH_BOUNDS);
        
        annotation { "Name" : "Z coordinate" }
        isLength(definition.ZCoord, LENGTH_BOUNDS);
    }
    {
        fLego(context, id + "fLego", {
            "origin" : vector(definition.XCoord, definition.YCoord, definition.ZCoord),
            "dimensions" : vector(definition.studCountX, definition.studCountY),
            "text" : "EVAN",
            });
    });