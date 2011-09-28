# -*- coding: utf-8 -*-
import uuid, cgi

from pprint import pprint

from webob import Request
from webob.exc import *
from selector import Selector
from beaker.middleware import SessionMiddleware
from paste.evalexception.middleware import EvalException

from xsltemplates import TemplateMiddleware, IndexXMLMiddleware
from xsltemplates import set_template, set_params

import openid
from openid.store import filestore
from openid.consumer import consumer
from openid.oidutil import appendArgs
from openid.store import memstore
from openid.cryptutil import randomString
from openid.fetchers import setDefaultFetcher, Urllib2Fetcher
from openid import sreg

import pkg_resources

from Cookie import SimpleCookie

# Thanks to WebOb source code :p
def make_cookie_header(key, value='', expires=-1):
    cookies = SimpleCookie()
    cookies[key] = value
    for var_name, var_value in [('max-age', 1296000), ('path', '/'), ('version', 1),
                                ('domain', 'dev.amp.fm'),]: # two weeks
        cookies[key][var_name] = str(var_value)
    if expires != -1:
        cookies[key]['expires'] = expires
    header_value = cookies[key].output(header='').lstrip()
    print header_value
    return ('Set-Cookie', header_value)
        
class OpenIdGateway(object):

    def __init__(self, app_conf):
        self.app_conf = app_conf
        self.sess_key = 'openid.amp.fm.gateway'        
        self.base_url = self.app_conf['base_url']
        self.ekey = 'openidgateway'
        self.urls = [
            ('[/]', {'GET': self.index}),
            ('/login[/]', {'GET': self.login, 'POST': self.login}),
            ('/complete[/]', {'GET': self.complete, 'POST': self.complete}),
            ('/transform/openid-redirect.xsl', {'GET': self.openid_redirect_xslt}),
        ]
        self._s = Selector(self.urls)
        if self.app_conf.get('debug', False):
            self._s.add('/forms[/]', {'GET': self.forms})
        self.store = filestore.FileOpenIDStore('./cache')

    def __call__(self, environ, start_response):
        return self._s(environ, start_response)

    def get_consumer(self, sess):
        return consumer.Consumer(sess, self.store)

    def index(self, environ, start_response):
        url = ''.join([self.base_url, 'login'])
        raise HTTPSeeOther(location=url)

    def login(self, environ, start_response):
        req = Request(environ)
        set_template(environ, 'login.xslt')
        sess = environ['beaker.session']
        params = {'base_uri': self.base_url}
        if not sess.get(self.ekey):
            sess[self.ekey] = {}
            sess.save()

        headers = [('Content-Type', 'application/xml')]
        if not (req.params.get('uname') or req.params.get('return_location')):
            message ='There must be a uname and return_location in the query string'
            raise HTTPBadRequest(detail=message)
        
        openid_url = req.params['uname']
        sess[self.ekey]['return_location'] = req.params['return_location']

        if not openid_url:
            # this seems better ...
            # raise HTTPBadRequest("need openid_url")
            params['message'] = "Don't leave your name blank."
            params['status'] = 'failure'
            set_params(environ, params)
            start_response('200 OK', headers)
            return []

        consumer = self.get_consumer(sess[self.ekey])
        try:
            request = consumer.begin(openid_url)
        except Exception, exc:
            params['message'] = 'Error in discovery: %s' % (cgi.escape(str(exc[0])))
            params['status'] = 'failure'
            set_params(environ, params)
            start_response('200 OK', headers)
            return []
        if request is None:
            errcode = cgi.escape(post['openid_url'])
            params['message'] = 'No OpenID services found for <code>%s</code>' % (errcode)
            params['status'] = 'failure'
            set_params(environ, params)            
            start_response('200 OK', headers)
            return []
        #sreg_request = sreg.SRegRequest(required=['nickname'])
        #request.addExtension(sreg_request)
        return_to = '%scomplete'% self.base_url
        return_to = '%s?identity=%s' % (return_to, openid_url)
        trusted_root = self.base_url
        
        sess[self.ekey]['trusted_root'] = trusted_root

        redirect_url = request.redirectURL(trusted_root, return_to)

        set_params(environ, {'redirect_url': redirect_url})
        params['status'] = 'redirect'
        params['message'] = 'OpendID Login Redirection'

        sess.save()
        set_params(environ, params)
        start_response('200 OK', headers)
        return []

    def complete(self, environ, start_response):
        req = Request(environ)
        sess = environ['beaker.session']
        set_template(environ, 'login.xslt')
        # params = {'base_uri': self.base_url}
        params = {'base_uri': 'http://dev.amp.fm/'}

        headers = [('Content-Type', 'application/xml')]

        consumer = self.get_consumer(sess[self.ekey])
        info = consumer.complete(req.GET)

        cookies = []
        if info.status == 'success':
            req.cookies['openid'] = req.params['identity']
            guid = req.cookies.get('guid', str(uuid.uuid1()))
            headers.append(make_cookie_header('openid.session', guid, expires=0))
            headers.append(make_cookie_header('openid', req.params['identity']))
            params['status'] = 'complete'
            params['return_location'] = sess[self.ekey]['return_location']
            params['message'] = 'Logged in as %s' % req.params['identity']
        elif info.status == 'failure':
            # Sylvain: I explicitely remove any existing cookie in case of a failure
            headers.append(make_cookie_header('openid.session', expires=0))
            headers.append(make_cookie_header('openid', expires=0))
            params['status']= 'failure'
            if info.identity_url:
                fmt = "Verification of %s failed: %s"                
                params['message'] = fmt % (cgi.escape(info.identity_url),
                                           info.message)
            else:
                params['message'] = "Verification failed"

        elif info.status == 'cancel':
            params['status'] = 'failure'
            params['message'] = 'Verfication cancelled'

        set_params(environ, params)
        start_response('200 OK', headers)
        return []

    def openid_redirect_xslt(self, environ, start_response):
        content = file('templates/openid-redirect.xsl', 'r').read()
        headers = [('Content-Type', 'text/xsl')]
        start_response('200 OK', headers)
        return [content]

    def forms(self, environ, start_response):
        form = '''<form id="openid-login" method="get" 
        action="%slogin" target="_top">
        <input id="openid-text" type="text"
               name="uname" class="single-input"
               maxlength="255"
               value="Authenticate w/ OpenID"
               onclick="if (this.value == 'Authenticate w/ OpenID') this.value = ''; return true;"
               onblur="if (this.value == '') this.value = 'Authenticate w/ OpenID'; return true;" />
        <input type="hidden" name="return_location" value="http://dev.amp.fm/" />
        <input id="openid-submit" class="single-input-submit" type="submit" value="Login" />
        </form>''' % self.base_url
        start_response('200 OK', [('Content-Type', 'text/html')])
        return ['<html><head><title>test forms</title></head>',
                '<body>', form, '</body></html>']




def make_openidgateway(app_conf):
    app = OpenIdGateway(app_conf)
    app = IndexXMLMiddleware(app_conf, app)
    app = TemplateMiddleware(app_conf, app)
    app = HTTPExceptionMiddleware(app)
    if app_conf.get('debug'):
        # each app should use this but for debugging/dev this is
        # appropriate
        app = EvalException(app)
    app = SessionMiddleware(app)
    return app
