def install(name):
    modname = 'cli.' + name
    module = getattr(__import__(modname), name)
    module.install()
