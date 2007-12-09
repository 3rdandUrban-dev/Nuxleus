var geo = {
   init : function() {
      var settings = {provider: "google",
                      maptype: "map",   
                      center: [40.72429917430525,-73.99598836898804],
                      zoom: 13,                         
                      controlsize: "small",
                      showtype: true,                   
                      showzoom: true,                   
                      showpan: true,                    
                      showoverview: true,               
                      showscale: true,          
                      dragging: true,                   
                      scrollzoom: true,        
                      smoothzoom: true, 
                      clickmarker: false
                    };
      //alert($('#gmap'));
      $("#gmap").jmap(settings);
      var gmap = $("#gmap").myMap();
      //alert(gmap);
      GEvent.addListener(gmap, "dblclick", function(marker, point){
         if (marker) {
            gmap.removeOverlay(marker);
         } else {
            var marker = new GMarker(point);
            gmap.addOverlay(marker);
            GEvent.addListener(marker, 'click', function(){
               pointlocation = marker.getPoint();
               marker.openInfoWindowHtml("Latitude: " + pointlocation.lat() + "<br />Longitude: " + pointlocation.lng());
               $('#ev_location').val(pointlocation.lat()+" "+pointlocation.lng());
            }); 
         }
      });
   },
   
   show_points: function(feeduri) {
     var gmap = $("#gmap").myMap();
     var geoxml = new GGeoXml(feeduri);
     gmap.addOverlay(geoxml);
   }
};
/*
$(document).ready(function(){
   geo.init();
});
*/
