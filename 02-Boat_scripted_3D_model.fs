FeatureScript 1096;
import(path : "onshape/std/geometry.fs", version : "1096.0");

export const mm = millimeter as ValueWithUnits;

export const BOAT_LENGTH_BOUNDS =
{
    (meter)      : [3, 10, 20],
    (centimeter) : 2.5,
    (millimeter) : 25.0,
    (inch)       : 1.0,
    (foot)       : 0.1,
    (yard)       : 0.025
} as LengthBoundSpec;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* sketchBoatXSection creates a sketch of a boat's cross-section
* on the given plane according to the given dimensions.
* 
* It takes arguments specifying the plane on which to sketch
* the cross-section, the width at the top of the sketch, the 
* width in the middle, the distance of the middle from the top, 
* the width near the bottom, the distance of the bottom from 
* the top, and the total height.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
function sketchBoatXSection(context is Context, id is Id, definition is map)
precondition
{
    annotation { "Name" : "Cross Section Plane" }
    definition.plane is Plane;
    
    annotation { "Name" : "Top Width" }
    isLength(definition.topWidth, NONNEGATIVE_LENGTH_BOUNDS);
    
    annotation { "Name" : "Middle Width" }
    isLength(definition.midWidth, NONNEGATIVE_LENGTH_BOUNDS);
    
    annotation { "Name" : "Middle Distance from Top" }
    isLength(definition.midDTop, NONNEGATIVE_LENGTH_BOUNDS);
    
    annotation { "Name" : "Bottom Width" }
    isLength(definition.botWidth, NONNEGATIVE_LENGTH_BOUNDS);
    
    annotation { "Name" : "Bottom Distance from Top" }
    isLength(definition.botDTop, NONNEGATIVE_LENGTH_BOUNDS);
    
    annotation { "Name" : "Height" }
    isLength(definition.height, NONNEGATIVE_LENGTH_BOUNDS);
}
{
    var XSSketch = newSketchOnPlane(context, id + "XSSketch", {
        "sketchPlane" : definition.plane
    });
        
        skFitSpline(XSSketch, "bowSplineR", {
                "points" : [
                    vector( definition.topWidth / 2 , 0 * mm ),
                    vector( definition.midWidth / 2 ,  -definition.midDTop ),
                    vector( definition.botWidth / 2 , -definition.botDTop ),
                    vector( 0 * mm,  -definition.height),
                ]
        });
        
        skFitSpline(XSSketch, "bowSplineL", {
                "points" : [
                    vector( -definition.topWidth / 2 , 0 * mm ),
                    vector( -definition.midWidth / 2 ,  -definition.midDTop ),
                    vector( -definition.botWidth / 2 , -definition.botDTop ),
                    vector( 0 * mm,  -definition.height),
                ]
        });
    
        skLineSegment(XSSketch, "bowTop", {
                "start" : vector( -definition.topWidth / 2, 0 * mm ),
                "end" : vector( definition.topWidth / 2, 0 * mm )
        });
            
    skSolve(XSSketch);
    return (XSSketch);
}


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* createBoat makes a model of a boat given a desired length 
* between 3 and 21 meters inclusive.
* 
* Sketches are constructed describing cross sections of the
* boat at the back, in the middle, and near the front. Another
* sketch is constructed describing the line of the keel.
* The body of the boat is created using a loft from the back 
* cross-section to the front, using the keel-line as a guide.
* Finally, the top surface is shelled out.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
annotation { "Feature Type Name" : "Create Boat" }
export const createBoat = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Boat Length" }
        isLength(definition.boatLength, BOAT_LENGTH_BOUNDS);
    }
    {
        // const mm                    is ValueWithUnits = millimeter;

        const length                is ValueWithUnits = definition.boatLength;
        const shellThickness        is ValueWithUnits = (7 / 3000) * length;

        const bowPlaneDFront        is ValueWithUnits = (639 / 3000) * length;
        const bowHeight             is ValueWithUnits = (695 / 3000) * length;
        const bowWidthTop           is ValueWithUnits = (660 / 3000) * length;
        const bowWidthMid           is ValueWithUnits = (620 / 3000) * length;
        const bowMidDTop            is ValueWithUnits = (313 / 3000) * length;
        const bowWidthBot           is ValueWithUnits = (450 / 3000) * length;
        const bowBotDTop            is ValueWithUnits = (539 / 3000) * length;

        const amidshipsPlaneDFront  is ValueWithUnits = (1828 / 3000) * length;
        const amidshipsHeight       is ValueWithUnits = (850 / 3000) * length;
        const amidshipsWidthTop     is ValueWithUnits = (1100 / 3000) * length;
        const amidshipsWidthMid     is ValueWithUnits = (983 / 3000) * length;
        const amidshipsMidDTop      is ValueWithUnits = (390 / 3000) * length;
        const amidshipsWidthBot     is ValueWithUnits = (483 / 3000) * length;
        const amidshipsBotDTop      is ValueWithUnits = (539 / 3000) * length;

        const sternPlaneDFront      is ValueWithUnits = (3000 / 3000) * length;
        const sternHeight           is ValueWithUnits = (860 / 3000) * length;
        const sternWidthTop         is ValueWithUnits = (990 / 3000) * length;
        const sternWidthMid         is ValueWithUnits = (940 / 3000) * length;
        const sternMidDTop          is ValueWithUnits = (308 / 3000) * length;
        const sternWidthBot         is ValueWithUnits = (408 / 3000) * length;
        const sternBotDTop          is ValueWithUnits = (539 / 3000) * length;
    

        // Sketch outlines for the front, middle, and back parts of the boat
        sketchBoatXSection(context, id + "bowSketch", {
            "plane" : plane( vector(0, (bowPlaneDFront / mm), 0) * mm, vector(0, -1, 0) * mm, vector(1, 0, 0) * mm ),
            "topWidth" : bowWidthTop,
            "midWidth" : bowWidthMid,
            "midDTop" : bowMidDTop,
            "botWidth" : bowWidthBot,
            "botDTop" : bowBotDTop,
            "height" : bowHeight,
            });
            
        sketchBoatXSection(context, id + "amidshipsSketch", {
            "plane" : plane( vector(0, (amidshipsPlaneDFront / mm), 0) * mm, vector(0, -1, 0) * mm, vector(1, 0, 0) * mm ),
            "topWidth" : amidshipsWidthTop,
            "midWidth" : amidshipsWidthMid,
            "midDTop" : amidshipsMidDTop,
            "botWidth" : amidshipsWidthBot,
            "botDTop" : amidshipsBotDTop,
            "height" : amidshipsHeight,
            });
            
        sketchBoatXSection(context, id + "sternSketch", {
            "plane" : plane( vector(0, (sternPlaneDFront / mm), 0) * mm, vector(0, -1, 0) * mm, vector(1, 0, 0) * mm ),
            "topWidth" : sternWidthTop,
            "midWidth" : sternWidthMid,
            "midDTop" : sternMidDTop,
            "botWidth" : sternWidthBot,
            "botDTop" : sternBotDTop,
            "height" : sternHeight,
            });
        
        
        // Sketch guide along the bottom of boat
        var keelSketch = newSketchOnPlane(context, id + "keelSketch", {
                "sketchPlane" : plane( vector(0, 0, 0) * mm, vector(1, 0, 0) * mm, vector(0, 1, 0) * mm )
        });
            
            skFitSpline(keelSketch, "keelSplineR", {
                    "points" : [
                        vector( 0 * mm , 0 * mm ),
                        vector( bowPlaneDFront , -bowHeight ),
                        vector( amidshipsPlaneDFront , -amidshipsHeight ),
                        vector( sternPlaneDFront, -sternHeight ),
                    ]
            });
        
        skSolve(keelSketch);


        // Loft from back through to front
        opLoft(context, id + "loftBoat", {
                "profileSubqueries" : [ 
                                        qSketchRegion(id + "sternSketch"),
                                        qSketchRegion(id + "amidshipsSketch"),
                                        qSketchRegion(id + "bowSketch"), 
                                        qCreatedBy(makeId("Origin"))
                                        ],
                "guideSubqueries" : [ qCreatedBy(id + "keelSketch", EntityType.EDGE) ]
        });


        // Shell out interior
        var face = qContainsPoint(qCreatedBy(id + "loftBoat", EntityType.FACE), vector(0, (bowPlaneDFront / mm) / 2, 0) * mm);
        opShell(context, id + "shellBoat", {
                "entities" : face,
                "thickness" : -shellThickness
        });
        
        // Delete sketches so they don't pollute the view
        opDeleteBodies(context, id + "deleteBodies1", {
                "entities" : qSketchFilter(qCreatedBy(id), SketchObject.YES)
        });
        
    });