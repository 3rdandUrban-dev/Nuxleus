//Creates the Silverlight object by calling into Silverlight.js.

function createSilverlight()

{  

    Silverlight.createObjectEx({source: 'simple.xaml', parentElement:pe, id:'agControl1', properties:{width:'1', height:'1', background:'white', isWindowless:'false', framerate:'24', version:'1.1'}, events:{onError:null, onLoad:null}, context:null});

    
}