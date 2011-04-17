
function doeHack(outputString)
{return outputString.replace(lt,"<").replace(gt,">").replace(amp,"&");};function myRouteHandler(route)
{var legs=route.RouteLegs;var turns="Turn-by-Turn Directions\n";var leg=null;var turnNum=0;var totalDistance=0;for(var i=0;i<legs.length;i++)
{leg=legs[i];var turn=null;var legDistance=null;for(var j=0;j<leg.Itinerary.Items.length;j++)
{turnNum++;turn=leg.Itinerary.Items[j];turns+=turnNum+":  "+turn.Text;legDistance=turn.Distance;totalDistance+=legDistance;turns+=" ("+legDistance.toFixed(1)+" miles)\n";}}
turns+="Total distance:  "+totalDistance.toFixed(1)+" miles\n";alert(turns);}
var count=10;function MoreResults(layer,resultsArray,places,hasMore,veErrorMessage)
{$("#results").fadeIn("slow");if(hasMore)
{var r="<a href='javascript:FindLoc(parseInt(10));'>"+"Click for More Results</a> (Showing ".concat(count).concat(' of '.concat(layer.GetShapeCount())).concat(' )');document.getElementById('results').innerHTML=r;count=count+10;}
else
{index=0;count=10;number=Number(10);document.getElementById('results').innerHTML="";document.getElementById('results').innerHTML="No More Results Available";window.setTimeout("$('#results').fadeOut('slow');",2000);}}
function outputVars(path){var perspective,timespan,view,location,filters;$.log("Path: ".concat(path));var view=path.split('/');var viewLength=view.length;for(var i=0;i<viewLength;i++){var current=view[i].split(':');switch(current[0]){case'perspective':perspective=(view[i])?view[i]:'home';break;case'timespan':timespan=(view[i])?view[i]:'timespan:today';break;case'view':viewspan=(view[i])?view[i]:'view:newspaper';break;case'location':location=view[i]
break;case'filters':filters=view[i];break;default:break;}}
$.log("Perspective: ".concat(perspective));$.log("Timespan: ".concat(timespan));$.log("View: ".concat(viewspan));$.log("Location: ".concat(location));$.log("Filters: ".concat(filters));return new Array(perspective,timespan,viewspan,location,filters);}