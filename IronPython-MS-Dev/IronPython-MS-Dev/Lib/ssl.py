# Copyright (c) 2006 Seo Sanghyeon

# 2006-04-19 sanxiyn Created

def _null_validation(*args):
    return True

def _make_ssl_stream_standard(stream):
    from System.Net.Security import SslStream
    ssl = SslStream(stream, True, _null_validation)
    ssl.AuthenticateAsClient('ignore')
    return ssl

def _make_ssl_stream_mono(stream):
    from Mono.Security.Protocol.Tls import SslClientStream
    ssl = SslClientStream(stream, 'ignore', False)
    ssl.ServerCertValidationDelegate = _null_validation
    return ssl

try:
    import clr
    clr.AddReference('Mono.Security')
except:
    _make_ssl_stream = _make_ssl_stream_standard
else:
    _make_ssl_stream = _make_ssl_stream_mono
