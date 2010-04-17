
function SarissaMediaWikiContext(apiUrl,arrLanguages){this.baseUrl=apiUrl;this.format="json";this.languages=arrLanguages;};SarissaMediaWikiContext.prototype.doArticleGet=function(sFor,callback){Sarissa.setRemoteJsonCallback(this.baseUrl+"?action=query&redirects&format="+
this.format+"&prop=revisions&rvprop=content&titles="+
encodeURIComponent(sFor),callback);};SarissaMediaWikiContext.prototype.doBacklinksGet=function(sFor,iLimit,callback){Sarissa.setRemoteJsonCallback(this.baseUrl+"?&generator=backlinks&format="+
this.format+"&gbllimit="+
iLimit+"&gbltitle"+
encodeURIComponent(sFor),callback);};SarissaMediaWikiContext.prototype.doSearch=function(sFor,iLimit,callback){Sarissa.setRemoteJsonCallback(this.baseUrl+"?action=query&list=search&srsearch="+
encodeURIComponent(sFor)+"&srwhat=text&srnamespace=0&format="+
this.format+"&srlimit="+
iLimit,callback);};SarissaMediaWikiContext.prototype.doCategorySearch=function(sFor,iLimit,callback){Sarissa.setRemoteJsonCallback(this.baseUrl+"?format="+
this.format+"&list=categorymembers&action=query&cmlimit="+
iLimit+"&cmtitle=Category:"+
encodeURIComponent(sFor),callback);};SarissaMediaWikiContext.prototype.doArticleCategoriesGet=function(sFor,iLimit,callback){Sarissa.setRemoteJsonCallback(this.baseUrl+"?format="+
this.format+"&action=query&prop=categories&titles="+
encodeURIComponent(sFor),callback);};