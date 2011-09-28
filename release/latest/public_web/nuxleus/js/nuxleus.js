var current = 1;
	
var IMGArray = new Array();
IMGArray[0] = "images/nuxleus-new-red-1.jpg";
IMGArray[1] = "images/nuxleus-new-blue-2.jpg";
IMGArray[2] = "images/nuxleus-new-green-1.jpg";
IMGArray[3] = "images/nuxleus-new-yellow-1.jpg";
IMGArray[4] = "images/nuxleus-new-blue-1.jpg";
IMGArray[5] = "images/nuxleus-new-blue-3.jpg";

function changeImages() {
   if (current >= IMGArray.length) current = 0;
   document.getElementById('nuxleus').src = IMGArray[current];
   current = current + 1;
   return true;
}

function setBaseImage (id) {
   var rand = Math.floor(Math.random() * IMGArray.length);
   document.getElementById(id).src = IMGArray[rand];
   return true;
}
