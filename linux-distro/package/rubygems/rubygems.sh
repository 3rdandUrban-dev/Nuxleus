# rubygems requires the following environment variable to work properly
export GEM_HOME=%(libdir)s/ruby/gems

# add rubygems bin directory to path for gem-generated wrappers
if ! echo ${PATH} | grep -q ${GEM_HOME}/bin ; then
    PATH=$PATH:$GEM_HOME/bin
fi
