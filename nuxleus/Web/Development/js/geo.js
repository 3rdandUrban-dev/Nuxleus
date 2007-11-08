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
     showEntries(geoxml);
   }

   showEntries: function(geoxml) {
    //var p;
	
    //	if (typeof this.entries != 'object')
    //		return;

    //	tl = this.entries.length;
    //	if (tl==0) { tl = 'no' }
    //	tleft = tl + ' new Entry' + (tl==1 ? '' : 's');
    //	$('entryCount').update(tleft);

    //	if (this.entries.length == 0) {
    //		this.loadEntries(0);
    //		return;
    //	}

	//	if ( p = this.entries.pop() ) {
	  //var marker = new GMarker(new GLatLng(p[0],p[1]), { icon : this.map_icon });
		// show photo
	  //	this.map.addOverlay(marker); 
	  //marker.openInfoWindowHtml(p[2]);
		
		// hide photo
		setTimeout( function () {
		    //tnu.sb.map.closeInfoWindow(); 
		    //tnu.sb.map.removeOverlay(marker);
			tnu.sb.showEntries(geoxml);
		}, 7000);
		//	}
   }
};
/*
$(document).ready(function(){
   geo.init();
});
*/
