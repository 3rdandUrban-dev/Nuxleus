
$(document).ready(function()
{var options={target:'#test',beforeSubmit:showRequest,success:showResponse,dataType:'xml',clearForm:true,resetForm:true};$("form[@class='ajaxForm']").submit(function()
{$(this).ajaxSend(function(evt,request,settings)
{request.setRequestHeader("Slug",this.slug.value);}).ajaxSubmit(options);return false;});$("a[@class='navigation']").click(function()
{var hash=this.href;var current=hash.replace(/^. * #/,'');var data_controller=this.rel.split(':');$('#test').getTransform('/page/controller/'.concat(data_controller[1]).concat('.xsl'),data_controller[0].concat('/atom.xml'),{params:{showModal:'1'},callback:function()
{}});return false;});});function showRequest(formData,jqForm,options)
{return true;}
function showResponse(responseXml,statusText)
{$('#test').getTransform('/page/controller/answer.xsl',responseXml,{params:{showModal:'1'},callback:function()
{}});return false;}
var serializer=new XMLSerializer();function transformDoc(xsltSrc,xmlDoc)
{var processor=new XSLTProcessor();var req=new XMLHttpRequest();req.open('GET',xsltSrc,true);req.overrideMimeType('text/xml');req.onreadystatechange=function(aEvt)
{if(req.readyState==4)
{if(req.status==200)
{processor.importStylesheet(req.responseXML);var transformResult=processor.transformToDocument(xmlDoc);var xmlSerializer=new XMLSerializer();var markup=xmlSerializer.serializeToString(transformResult);alert(markup);return false;}}};req.send(null);}