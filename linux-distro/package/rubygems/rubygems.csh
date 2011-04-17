# rubygems requires the following environment variable to work properly
setenv GEM_HOME %(libdir)s/ruby/gems

# add rubygems bin directory to path for gem-generated wrappers
if ( "${path}" !~ *${GEM_HOME}/bin* ) then
    set path = ( $path $GEM_HOME/bin )
endif

