# -*- coding: utf-8 -*-

import sys
import codecs
import md5
import time
import os, os.path
import hmac
import hashlib
import random
from urllib import quote, unquote
from urlparse import urlparse, urljoin
from datetime import datetime, timedelta

import cherrypy
from cherrypy.lib.static import serve_file
from cherrypy.lib.http import parse_query_string
import boto
from boto.s3.key import Key
from memcache import Client
import openid
from openid import sreg
from openid.server import server
from openid.store.filestore import FileOpenIDStore
from openid.consumer import discover
from Crypto.Cipher import AES

current_dir = os.getcwd()

OID_COOKIE_SECRET_KEY = hmac.new(file('./cookie.key').readline().strip()).hexdigest()

HOST = "amp.fm"
OID_HOST = "openid.amp.fm"

BASE_URL =  "http://openid.%s" % HOST
BASE_SECURE_URL = "http://openid.%s" % HOST
TRUST_URL =  "http://openid.%s/trust" % HOST
OID_SERVICE_URL = "http://openid.%s/service" % HOST

# From CherryPy
try:
    os.urandom(20)
except (AttributeError, NotImplementedError):
    # os.urandom not available until Python 2.4. Fall back to random.random.
    def generate_unique():
        return md5.new('%s' % random.random()).hexdigest()
else:
    def generate_unique():
        return os.urandom(16).encode('hex')

def extract_best_charset():
    charsets = cherrypy.request.headers.elements('Accept-Charset')
    if charsets:
        try:
            encoding = codecs.lookup(charsets[0].value)
            return encoding.name
        except LookupError:
            pass
            
    return 'ISO-8859-1'

def extract_login(url):
    return urlparse(url)[2].strip('/')

def redirect_to_login_page(from_url):
    raise cherrypy.HTTPRedirect(cherrypy.url('/login?redirect_to=%s' % quote(from_url))) 

def check_login(s3conn, mc, from_url):
    cookie = cherrypy.request.cookie.get('openid.name', None)
    if not cookie:
        raise redirect_to_login_page(from_url)

    login, session_id = read_values_from_cookie(cookie)
    if login == None or session_id == None:
        raise redirect_to_login_page(from_url)

    stored_session_id = mc.get('%s.openid.name.cookie' % login)
    if stored_session_id:
        if stored_session_id != session_id:
            raise redirect_to_login_page(from_url)
    else:
        b = s3conn.create_bucket('openid.amp.fm')
        k = Key(b)
        k.key = '%s/cookie' % login
        if k.exists():
            if k.get_contents_as_string() != session_id:
                raise redirect_to_login_page(from_url)

    return login

def pad_str(text, size=16):
    text_len = len(text)
    if text_len == size:
        return text
    
    if text_len < size:
        return text + '*' * (size - text_len)

    return text + '*' * (size - (text_len % size))

def unpad_str(text, size=16):
    return text.rstrip('*')
    
def make_cookie_values(username):
    padded_username = pad_str(username, size=15)
    session_id = generate_unique()
    aes = AES.new(OID_COOKIE_SECRET_KEY, AES.MODE_ECB)
    return session_id, aes.encrypt('%s,%s' % (padded_username, session_id))

def read_values_from_cookie(cookie):
    aes = AES.new(OID_COOKIE_SECRET_KEY, AES.MODE_ECB)
    value = aes.decrypt(cookie.value)
    try:
        padded_username, session_id = value.rsplit(',', 1)
        return unpad_str(padded_username), session_id
    except ValueError:
        return None, None

# we disallow the following terms to be used as login values
_reserved_login = ['trust', 'signup', 'login', 'logout', 'service', 'revoke']

class OpenIDAccountSignUpHandler(object):
    exposed = True

    def __init__(self, s3conn):
        self.s3conn = s3conn

    def GET(self):
        return serve_file(os.path.join(current_dir, 'signupGET.xml'),
                          content_type='application/xml')

    def POST(self, login, password, confirm_password):
        cherrypy.response.headers['content-type'] = 'application/xml'
        if login in _reserved_login:
            error_body = file(os.path.join(current_dir, 'signupPOSTerror.xml')).read()
            return error_body % "This login cannot be used"

        if password != confirm_password:
            error_body = file(os.path.join(current_dir, 'signupPOSTerror.xml')).read()
            return error_body % "Passwords do not match"

        # Here we suppose that the best acceptable charset for the client
        # is also the one it used to encode the data sent
        charset = extract_best_charset()
        login =  quote(login.decode(charset).encode('utf-8'))

        oid = login
        b = self.s3conn.create_bucket('openid.amp.fm')
        k = Key(b)
        k.key = oid

        if k.exists() == True:
            error_body = file(os.path.join(current_dir, 'signupPOSTerror.xml')).read()
            return error_body % "This login is already used"

        k.set_contents_from_string(md5.new(password).hexdigest())

        #return "Your identifier is %s" % urljoin(BASE_URL, oid)
        success_body = file(os.path.join(current_dir, 'signupPOSTsuccess.xml')).read()
        return success_body % urljoin(BASE_URL, oid)
             
class OpenIDLogoutHandler(object):
    exposed = True

    def __init__(self, s3conn, mc):
        self.s3conn = s3conn
        self.mc = mc

    def GET(self):
        return serve_file(os.path.join(current_dir, 'logoutGET.xml'),
                          content_type='application/xml')
    
    def POST(self):
        cookie = cherrypy.request.cookie.get('openid.name', None)
        if cookie:
            login, session_id = read_values_from_cookie(cookie)
            if login != None and session_id != None:
                self.mc.delete('%s.openid.name.cookie' % login)
                b = self.s3conn.create_bucket('openid.amp.fm')
                b.delete_key('%s/cookie' % login)

        cherrypy.response.cookie['openid.name'] = ''
        cherrypy.response.cookie['openid.name']['path'] = '/'
        cherrypy.response.cookie['openid.name']['version'] = 1
        cherrypy.response.cookie['openid.name']['expires'] = 0
        cherrypy.response.cookie['openid.name']['domain'] = 'openid.amp.fm'

        raise cherrypy.HTTPRedirect('http://dev.amp.fm/')

class OpenIDLoginHandler(object):
    exposed = True

    def __init__(self, s3conn, mc):
        self.s3conn = s3conn
        self.mc = mc

    def GET(self, redirect_to=None):
        cherrypy.response.headers['content-type'] = 'application/xml'
        login_form = file(os.path.join(current_dir, 'loginGET.xml')).read()
        login = ''
        if redirect_to:
            urlqs = parse_query_string(unquote(redirect_to))
            uname = urlqs.get('uname', None)
            if uname:
                login = uname.split('%s/' % OID_HOST)[-1]
        return login_form % (quote(redirect_to or ''), quote(login))

    def POST(self, login, password, redirect_to=None):
        cherrypy.response.headers['content-type'] = 'application/xml'
        redirect_to = redirect_to or ''
        oid = login
        b = self.s3conn.create_bucket('openid.amp.fm')
        k = Key(b)
        k.key = oid

        if k.exists() == False:
            error_body = file(os.path.join(current_dir, 'loginPOSTerror.xml')).read()
            return error_body % (redirect_to,
                                 "The provided login does not exist")

        hashed_pwd = k.get_contents_as_string()
        if hashed_pwd != md5.new(password).hexdigest():
            error_body = file(os.path.join(current_dir, 'loginPOSTerror.xml')).read()
            return error_body % (redirect_to,
                                 "The provided password is not correct")

        session_id, cookie_val = make_cookie_values(login)
        cherrypy.response.cookie['openid.name'] = cookie_val
        cherrypy.response.cookie['openid.name']['path'] = '/'
        cherrypy.response.cookie['openid.name']['max-age'] = 31536000 # 1 year
        #cherrypy.response.cookie['openid.name']['secure'] = 1
        cherrypy.response.cookie['openid.name']['version'] = 1
        cherrypy.response.cookie['openid.name']['domain'] = 'openid.amp.fm'

        self.mc.set('%s.openid.name.cookie' % login, session_id)
        k = Key(b)
        k.key = '%s/cookie' % login
        k.set_contents_from_string(session_id)

        if redirect_to not in (None, ''):
            raise cherrypy.HTTPRedirect(unquote(redirect_to))
        
        success_body = file(os.path.join(current_dir, 'loginPOSTsuccess.xml')).read()
        return success_body % login

class OpenIDRevokeHandler(object):
    exposed = True

    def __init__(self, s3conn, mc):
        self.s3conn = s3conn
        self.mc = mc

    def GET(self):
        login = check_login(self.s3conn, self.mc, cherrypy.url())
        b = self.s3conn.create_bucket('openid.amp.fm')
        k = Key(b)
        k.key = '%s/trusted' % login
        trusted = None
        if k.exists():
            trusted = k.get_contents_as_string()

        if trusted:
            trusted = trusted.split('\r\n')
            body = """<html>
<head />
<body>
<p>You can revoke the following trusted parties:</p>
<form name="revoke_trust" action="%s/revoke" method="post">""" %  BASE_SECURE_URL
        
            pos = 0
            for trust in trusted:
                if trust:
                    body = body + """<input type="checkbox" name="trustee%d" value="%s">%s</input><br />""" % (pos, quote(trust), trust)
                    pos += 1
                
            return body + """<br /><input type="submit" value="Revoke" /></form></body></html>"""
        
        return "You have no trustees to revoke"

    def POST(self, *args, **kwargs):
        login = check_login(self.s3conn, self.mc, cherrypy.url())
        b = self.s3conn.create_bucket('openid.amp.fm')
        k = Key(b)
        k.key = '%s/trusted' % login
        trusted = None
        if k.exists():
            trusted = k.get_contents_as_string()
        
        if trusted:
            trusted = trusted.split('\r\n')

            for key in kwargs:
                if key.startswith('trustee'):
                    trustee = unquote(kwargs[key])
                    if trustee in trusted:
                        trusted.remove(trustee)

            k.set_contents_from_string('\r\n'.join(trusted))

        raise cherrypy.HTTPRedirect(cherrypy.url())
        
class OpenIDEndPointHandler(object):
    exposed = True

    def __init__(self, s3conn, mc, oidserver):
        self.s3conn = s3conn
        self.oidserver = oidserver
        self.mc = mc 

    def POST(self, *args, **kwargs):
        if 'request_id' in kwargs:
            request = self.mc.get(kwargs['request_id'])
            if request:
                login = extract_login(request.identity)
                # Let's make sure the request is removed from our cache
                self.mc.delete(kwargs['request_id'])
                if 'allow_once' in kwargs or 'allow_forever' in kwargs:
                    # In the following case we store on S3 the 
                    # trusted root URL so that the user won't have to manually 
                    # allow it in the future
                    if 'allow_forever' in kwargs:
                        b = self.s3conn.create_bucket('openid.amp.fm')
                        k = Key(b)
                        k.key = '%s/trusted' % login
                        trusted = [request.trust_root]
                        if k.exists():
                            _trusted = k.get_contents_as_string()
                            if _trusted:
                                trusted = _trusted.split('\r\n')
                                if request.trust_root not in trusted:
                                    trusted.append(request.trust_root)
                        k.set_contents_from_string('\r\n'.join(trusted))
                    response = request.answer(True, identity=request.identity)
                    response = self.oidserver.encodeResponse(response)
                    cherrypy.response.status = response.code
                    cherrypy.response.headers.update(response.headers)
                    return response.body
                if 'deny' in kwargs:
                    response = request.answer(False)
                    response = self.oidserver.encodeResponse(response)
                    cherrypy.response.status = response.code
                    cherrypy.response.headers.update(response.headers)
                    return response.body
        
        raise cherrypy.HTTPRedirect(cherrypy.url('/'))

class OpenIDRoot(object):
    def __init__(self, oidserver, s3conn, mc):
        self.oidserver = oidserver
        self.s3conn = s3conn
        self.mc = mc

        # Handlers for the different OpenID service URIs
        self.trust = OpenIDEndPointHandler(self.s3conn, self.mc, self.oidserver)
        self.login = OpenIDLoginHandler(self.s3conn, self.mc)
        self.logout = OpenIDLogoutHandler(self.s3conn, self.mc)
        self.signup = OpenIDAccountSignUpHandler(s3conn)
        self.revoke = OpenIDRevokeHandler(self.s3conn, self.mc)

    @cherrypy.expose
    def index(self):
        return "hello"

    @cherrypy.expose
    def default(self, oid):
        return """<html><head>
<link rel="openid.server" href="%s" />
<link rel="openid2.server" href="%s" />
<meta http-equiv="x-xrds-location" content="%s/yadis/%s" />
</head>
<body>
Hi %s
</body>
</html>
""" % (OID_SERVICE_URL, OID_SERVICE_URL, BASE_SECURE_URL, oid, oid)

    @cherrypy.expose
    def yadis(self, login):
        cherrypy.response.headers['content-type'] = 'application/xrds+xml'
        return """\
<?xml version="1.0" encoding="UTF-8"?>
<xrds:XRDS xmlns:xrds="xri://$xrds" xmlns="xri://$xrd*($v*2.0)" xmlns:openid="http://openid.net/xmlns/1.0">
  <XRD>

    <Service priority="0">
      <Type>%s</Type>
      <URI>%s</URI>
      <openid:Delegate>%s/%s</openid:Delegate>
    </Service>

    <Service priority="20">
      <Type>%s</Type>
      <URI>%s</URI>
      <openid:Delegate>%s/%s</openid:Delegate>
    </Service>

  </XRD>
</xrds:XRDS>
"""%(discover.OPENID_2_0_TYPE, OID_SERVICE_URL, BASE_SECURE_URL, login,
     discover.OPENID_1_0_TYPE, OID_SERVICE_URL, BASE_SECURE_URL, login)

    @cherrypy.expose
    def service(self, *args, **kwargs):
        try:
            request = self.oidserver.decodeRequest(cherrypy.request.params)
        except server.ProtocolError, pe:
            cherrypy.log(str(pe))
            return "An error occurred while decoding the request"

        # In the following case we are handling the confirmation
        # of the identity of the user
        if request.mode in ["checkid_authentication"]:
            login = extract_login(request.identity)

            b = self.s3conn.create_bucket('openid.amp.fm')
            k = Key(b)
            k.key = login

            # Here we have been passed an identiy that the system does not
            # know. We just fail the authentication immediatly.
            if k.exists() == False:
                response = request.answer(False)
                response = self.oidserver.encodeResponse(response)
                cherrypy.response.status = response.code
                cherrypy.response.headers.update(response.headers)
                return response.body

            # We check to see if the trust root has already been
            # allowed by the user. If yes, we immediatly return the valid response
            k = Key(b)
            k.key = '%s/trusted' % login
            if k.exists():
                trusted = k.get_contents_as_string()
                trusted = trusted.split('\r\n')
                if request.trust_root in trusted:
                    response = request.answer(True, identity=request.identity)
                    response = self.oidserver.encodeResponse(response)
                    cherrypy.response.status = response.code
                    cherrypy.response.headers.update(response.headers)
                    return response.body

        # If we are in one of those modes, it means we are
        # at the beginning of the process and we must ask the user
        # what he or she wants to do
        elif request.mode in ["checkid_immediate", "checkid_setup"]:

            # First we check to see if the user is logged in
            login = check_login(self.s3conn, self.mc, cherrypy.request.headers['referer'])
            if extract_login(request.identity) != login:
                raise redirect_to_login_page(cherrypy.request.headers['referer'])
            
            # We check to see if the trust root has already been
            # allowed by the user. If yes, we immediatly return the valid response
            b = self.s3conn.create_bucket('openid.amp.fm')
            k = Key(b)
            k.key = '%s/trusted' % login
            if k.exists():
                trusted = k.get_contents_as_string()
                trusted = trusted.split('\r\n')
                if request.trust_root in trusted:
                    response = request.answer(True, identity=request.identity)
                    response = self.oidserver.encodeResponse(response)
                    cherrypy.response.status = response.code
                    cherrypy.response.headers.update(response.headers)
                    return response.body

            # First let's compute a request id to track this request
            request_id = generate_unique()

            # We store the current request to follow-up the process
            self.mc.set(request_id, request)
            
	    cherrypy.response.headers['content-type'] = 'application/xml'
            service_form = file(os.path.join(current_dir, 'serviceGET.xml')).read()
            return  service_form % (request.identity, request.trust_root, request_id)
        else:
            # All good, the authentication can be completed!
            response = self.oidserver.handleRequest(request)
            response = self.oidserver.encodeResponse(response)
            cherrypy.response.status = response.code
            cherrypy.response.headers.update(response.headers)
            return response.body



if __name__ == "__main__":
    cherrypy.config.update({'engine.autoreload_on' : False,
                            'server.socket_port' : 8090, 
                            'server.socket_host': '127.0.0.1',
                            'server.socket_queue_size': 25,
                            #'server.ssl_certificate': './server.crt', 
                            #'server.ssl_private_key': './server.key',
                            'log.screen': False,
                            'log.access_file': './access.log',
                            'log.error_file': './error.log',
                            'checker.on': False,
                            'tools.proxy.on': True,
                            'tools.proxy.base': 'http://openid.%s' % HOST})

    from cherrypy import dispatch
    method_disp = dispatch.MethodDispatcher()
    conf = {'/': { 'tools.gzip.on': True ,
                   'tools.etags.on': True,
                   'tools.etags.autotags': True },
            '/login': { 'request.dispatch': method_disp },
            '/logout': { 'request.dispatch': method_disp },
            '/signup': { 'request.dispatch': method_disp },
            '/revoke': { 'request.dispatch': method_disp },
            '/trust': { 'request.dispatch': method_disp }}

    mc = Client(['127.0.0.1:11211'])

    s3conn = boto.connect_s3(file('./s3pub.key').readline().strip(),
                             file('./s3priv.key').readline().strip())

    store = FileOpenIDStore('./cache')
    oidserver = server.Server(store, OID_SERVICE_URL)
    oidapp = OpenIDRoot(oidserver, s3conn, mc)

    cherrypy.quickstart(oidapp, '/', config=conf)
