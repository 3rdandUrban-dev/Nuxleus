var blog = {
   init : function() {
      $('#eventform').bind("submit", null, blog.post_event);
   },

   post_event : function(event) {
      var qs = '';
      qs += 'name='+encodeURIComponent($('#name').val())+'&';
      qs += 'description='+encodeURIComponent($('#description').val())+'&';
      qs += 'location='+encodeURIComponent($('#location').val())+'&';
      qs += 'startdate='+encodeURIComponent($('#startdate').val())+'&';
      qs += 'enddate='+encodeURIComponent($('#enddate').val())+'&';
      qs += 'genre='+encodeURIComponent($('#genre').val())+'&';
      qs += 'tags='+encodeURIComponent($('#tags').val());

      $.ajax({type: 'POST',
	      url: "/blog/pub/event",
	      dataType:  'xml', 
	      data: qs,
	      success: function(atom_entry) {
		 window.location = "/blog/event";
	      }
	     });
      event.stopPropagation();
				       
      return false;
   }
};

$(document).ready(function(){
   blog.init();
});