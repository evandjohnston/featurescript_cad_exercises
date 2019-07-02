FeatureScript 1096;
import(path : "onshape/std/geometry.fs", version : "1096.0");

export const mm = millimeter as ValueWithUnits;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* findLineIntersection finds the intersection point of two lines
* given their starting and ending points.
*
* It takes arguments of 2D vectors for the starting and ending
* points of 2 lines, identified as A and B.
*
* This function is useful in cases where the user does not wish
* to draw both complete lines, as is usually necessary to
* find the intersection point using built-in functions:
* https://forum.onshape.com/discussion/6814/point-of-intersection
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function findLineIntersection(startA is Vector, endA is Vector, startB is Vector, endB is Vector)
{
    var x;
    var y;
    
    x = ( (startA[0] * endA[1] - startA[1] * endA[0]) * (startB[0] - endB[0]) - (startA[0] - endA[0]) * (startB[0] * endB[1] - startB[1] * endB[0]) )
        / ((startA[0] - endA[0]) * (startB[1] - endB[1]) - (startA[1] - endA[1]) * (startB[0] - endB[0]));

    y = ( (startA[0] * endA[1] - startA[1] * endA[0]) * (startB[1] - endB[1]) - (startA[1] - endA[1]) * (startB[0] * endB[1] - startB[1] * endB[0]) )
        / ( (startA[0] - endA[0]) * (startB[1] - endB[1]) - (startA[1] - endA[1]) * (startB[0] - endB[0]) );
    
    return (vector(x, y));
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* drawRotatedLine draws a line copied up to 4 times rotated at 
* intervals of 90º around the origin.
*
* It takes arguments of the sketch on which to draw, the name for
* the lines, the ratio of the basic x unit divided by the basic y
* unit, the boolean value specifying whether the lines should be
* drawn as construction lines, an array specifying the axes to
* mirror and swap across, and the starting and ending coordinates
* of the desired line.
*
* This function is useful in cases where sketch lines must be
* rotated before the sketch is solved, since there is no function
* for rotating unresolved sketches.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function drawRotatedLine(
                            sketch is Sketch, skName is string, xy_ratio is number,
                            construction is boolean, mirrorAxes is array,
                            start is Vector, end is Vector
                            )
{
    if (mirrorAxes[0] == true) {
        skLineSegment(sketch, skName~"Q1", {
                "start" : vector( start[0], start[1] ),
                "end" : vector( end[0], end[1] ),
                "construction" : construction
        });
    }
    
    if (mirrorAxes[1] == true) {
        skLineSegment(sketch, skName~"Q2", {
                "start" : vector( -start[1] * xy_ratio, start[0] / xy_ratio ),
                "end" : vector( -end[1] * xy_ratio, end[0] / xy_ratio ),
                "construction" : construction
        });
    }
    
    if (mirrorAxes[2] == true) {
        skLineSegment(sketch, skName~"Q3", {
                "start" : vector( -start[0], -start[1] ),
                "end" : vector( -end[0], -end[1] ),
                "construction" : construction
        });
    }
    
    if (mirrorAxes[3] == true) {
        skLineSegment(sketch, skName~"Q4", {
                "start" : vector( start[1] * xy_ratio, -start[0] / xy_ratio ),
                "end" : vector( end[1] * xy_ratio, -end[0] / xy_ratio ),
                "construction" : construction
        });
    }
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* createStar draws a geometric pattern depicting an 8-pointed star 
* with given unit lengths in the x and y dimensions, and with or 
* without gridlines in each of the four quadrants.
*
* It accepts parameters describing the basic grid lengths in the x
* and y dimensions, and boolean values specifying whether or not
* to display construction gridlines in each of the four quadrants.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
annotation { "Feature Type Name" : "Create Star" }
export const createStar = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Grid Unit Width" }
        isLength(definition.gridX, NONNEGATIVE_LENGTH_BOUNDS);
        
        annotation { "Name" : "Grid Unit Height" }
        isLength(definition.gridY, NONNEGATIVE_LENGTH_BOUNDS);
        
        annotation { "Name" : "Q1 gridlines", "Default" : true }
        definition.gridQ1 is boolean;
        
        annotation { "Name" : "Q2 gridlines", "Default" : false }
        definition.gridQ2 is boolean;
        
        annotation { "Name" : "Q3 gridlines", "Default" : false }
        definition.gridQ3 is boolean;
        
        annotation { "Name" : "Q4 gridlines", "Default" : false }
        definition.gridQ4 is boolean;
        
    }
    {
        const x         is ValueWithUnits = definition.gridX;
        const y         is ValueWithUnits = definition.gridY;
        const xy_ratio  is number = x/y;
        
        var baseSketch is Sketch = newSketch(context, id + "baseSketch", {
            "sketchPlane" : qCreatedBy(makeId("Front"), EntityType.FACE)
        });
            
            // Draw gridlines
            var gridMirrorAxes is array = [definition.gridQ1, definition.gridQ2, definition.gridQ3, definition.gridQ4];
            for (var i = 0; i < 4; i += 1) {
                drawRotatedLine(baseSketch, "gridh"~i, xy_ratio, true, gridMirrorAxes, vector(0 * x, i * y), vector(3 * x, i * y));
                drawRotatedLine(baseSketch, "gridv"~i, xy_ratio, true, gridMirrorAxes, vector(i * x, 0 * y), vector(i * x, 3 * y));
            }


            // Draw image lines
            var imageMirrorAxes is array = [true, true, true, true];
            var intersectPt is Vector = vector(0, 0, 0);
            intersectPt = findLineIntersection(vector(0 * x, 3 * y), vector(3 * x, 0 * y), vector(1 * x, 0 * y), vector(2 * x, 2 * y));
            drawRotatedLine(baseSketch, "sq-top-1", xy_ratio, false, imageMirrorAxes, vector(0 * x, 3 * y), intersectPt);
            
            intersectPt = findLineIntersection(vector(0 * x, 3 * y), vector(3 * x, 0 * y), vector(2 * x, 0 * y), vector(3 * x, 3 * y));
            drawRotatedLine(baseSketch, "sq-top-2", xy_ratio, false, imageMirrorAxes, vector(3 * x, 0 * y), intersectPt);
            
            intersectPt = findLineIntersection(vector(0 * x, 2 * y), vector(2 * x, 0 * y), vector(1 * x, 0 * y), vector(2 * x, 2 * y));
            drawRotatedLine(baseSketch, "sq-bot-1", xy_ratio, false, imageMirrorAxes, vector(0 * x, 2 * y), intersectPt);
            
            intersectPt = findLineIntersection(vector(0 * x, 2 * y), vector(3 * x, 3 * y), vector(0 * x, 3 * y), vector(3 * x, 0 * y));
            drawRotatedLine(baseSketch, "st-htop-1", xy_ratio, false, imageMirrorAxes, intersectPt, vector(3 * x, 3 * y));
            
            intersectPt = findLineIntersection(vector(0 * x, 1 * y), vector(2 * x, 2 * y), vector(0 * x, 2 * y), vector(2 * x, 0 * y));
            drawRotatedLine(baseSketch, "st-hbot-1", xy_ratio, false, imageMirrorAxes, vector(0 * x, 1 * y), intersectPt);
            
            intersectPt = findLineIntersection(vector(0 * x, 1 * y), vector(2 * x, 2 * y), vector(0 * x, 3 * y), vector(3 * x, 0 * y));
            drawRotatedLine(baseSketch, "st-hbot-2", xy_ratio, false, imageMirrorAxes, intersectPt, vector(2 * x, 2 * y));
            
            drawRotatedLine(baseSketch, "st-vleft", xy_ratio, false, imageMirrorAxes, vector(2 * x, 2 * y), vector(1 * x, 0 * y));
            
            drawRotatedLine(baseSketch, "st-vright", xy_ratio, false, imageMirrorAxes, vector(3 * x, 3 * y), vector(2 * x, 0 * y));
            
        skSolve(baseSketch);
    
    });