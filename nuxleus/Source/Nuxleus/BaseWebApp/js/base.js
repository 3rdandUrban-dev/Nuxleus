$(document) .ready(function ()
{
    var options =
    {
        target: '#test', // target element(s) to be updated with server response
        beforeSubmit:  showRequest, // pre-submit callback
        success:       showResponse, // post-submit callback
        
        // other available options:
        //url:       url         // override for form's 'action' attribute
        //type:      type        // 'get' or 'post', override for form's 'method' attribute
        dataType: 'xml', // 'xml', 'script', or 'json' (expected server response type)
        clearForm: true, // clear all form fields after successful submit
        resetForm: true // reset the form after successful submit
        
        // $.ajax options can be used here too, for example:
        //timeout:   3000
    };
    
    $("form[@class='ajaxForm']") .submit(function ()
    {
        $(this) .ajaxSend(function (evt, request, settings)
        {
            request.setRequestHeader("Slug", this .slug.value);
        }) .ajaxSubmit(options);
        return false;
    });
    
    $("a[@class='vote']").click(function () {
        $.post("/service/pub/vote/", { id: this.href, vote: this.rel },
            function (data) {
                alert("Data recieved: " + data);
            });
          return false;
    });
    
    $("a[@class='navigation']") .click(function ()
    {
        var hash = this .href;
        var current = hash.replace(/^. * #/, '');
        var data_controller = this .rel.split(':');
        $('#test') .getTransform('/page/controller/' .concat(data_controller[1]) .concat('.xsl'), data_controller[0].concat('/atom.xml'),
        {
params:
            {
showModal: '1'
            },
            callback: function ()
            {
            }
        });
        return false;
    });
});

// pre-submit callback
function showRequest(formData, jqForm, options)
{
    return true;
}

function showResponse(responseXml, statusText)
{
    $('#test') .getTransform('/page/controller/answer.xsl', responseXml,
    {
params:
        {
showModal: '1'
        },
        callback: function ()
        {
        }
    });
    
    //var serializer = new XMLSerializer();
    
    //var xmlString = serializer.serializeToString(responseXml);
    //transformDoc('/page/controller/answer.xsl', responseXml);
    
    return false;
}

var serializer = new XMLSerializer();

function transformDoc(xsltSrc, xmlDoc)
{
    var processor = new XSLTProcessor();
    var req = new XMLHttpRequest();
    req.open('GET', xsltSrc, true);
    req.overrideMimeType('text/xml');
    req.onreadystatechange = function (aEvt)
    {
        if (req.readyState == 4)
        {
            if (req.status == 200)
            {
                
                processor.importStylesheet(req.responseXML);
                
                var transformResult = processor.transformToDocument(xmlDoc);
                var xmlSerializer = new XMLSerializer();
                // serialize
                var markup = xmlSerializer.serializeToString(transformResult);
                alert(markup);
                return false;
            }
        }
    };
    req.send(null);
}