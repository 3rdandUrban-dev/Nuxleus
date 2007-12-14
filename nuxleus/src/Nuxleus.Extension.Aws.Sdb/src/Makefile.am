CSC = $(MCS) $(MCS_FLAGS) $(CSFLAGS)

ASSEMBLY_NAME=sdb-sharp

PACKAGE_SHARP=sdb-sharp
ASSEMBLY=$(ASSEMBLY_NAME).dll
TARGET=$(ASSEMBLY)

KEYFILE=$(srcdir)/sdb-sharp.keys

SDB_CSFILES = \
	$(srcdir)/Attribute.cs \
	$(srcdir)/Domain.cs \
	$(srcdir)/HttpQueryConnection.cs \
	$(srcdir)/IAwsConnection.cs \
	$(srcdir)/Item.cs \
	$(srcdir)/GetAttributesResponse.cs \
	$(srcdir)/ListDomainsResponse.cs \
	$(srcdir)/ListResponse.cs \
	$(srcdir)/QueryResponse.cs \
	$(srcdir)/Response.cs \
	$(srcdir)/SdbConnection.cs \
	$(srcdir)/SdbException.cs \
	$(srcdir)/Sdb.cs

SDB_ASSEMBLIES = \
	-r:System.Web

$(ASSEMBLY): $(SDB_CSFILES)
	$(CSC) /out:$(ASSEMBLY) -keyfile:$(KEYFILE) /target:library \
		$(SDB_ASSEMBLIES) \
		$(SDB_CSFILES)

BASIC_SAMPLE=basic_sample
BASIC_SAMPLE_TARGET=basic_sample.exe
BASIC_SAMPLE_CSFILES = \
	$(SDB_CSFILES) \
	$(srcdir)/BasicSample.cs

CLEANFILES = $(ASSEMBLY) $(BASIC_SAMPLE_TARGET)
noinst_DATA = $(ASSEMBLY) $(BASIC_SAMPLE_TARGET)

$(BASIC_SAMPLE_TARGET): $(BASIC_SAMPLE_CSFILES)
	$(CSC) -out:$@ $(SDB_ASSEMBLIES) $(BASIC_SAMPLE_CSFILES)
#        $(CSC) -out:$@ $(SDB_ASSEMBLIES) $(BASIC_SAMPLE_CSFILES)

install-data-local:
	@if test -n '$(TARGET)'; then									\
	  if test -n '$(DESTDIR)'; then								\
	    echo "$(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE_SHARP) /root $(DESTDIR)$(prefix)/lib";		\
	    $(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE_SHARP) /root $(DESTDIR)$(prefix)/lib || exit 1;	\
	  else												\
	    echo "$(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE_SHARP) /gacdir $(prefix)/lib";			\
	    $(GACUTIL) /i $(ASSEMBLY) /f /package $(PACKAGE_SHARP) /gacdir $(prefix)/lib || exit 1;		\
	  fi;												\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then									\
	  if test -n '$(DESTDIR)'; then								\
	    echo "$(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE_SHARP) /root $(DESTDIR)$(prefix)/lib";	\
	    $(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE_SHARP) /root $(DESTDIR)$(prefix)/lib || exit 1;  \
	  else												\
	    echo "$(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE_SHARP) /gacdir $(prefix)/lib";	\
	    $(GACUTIL) /u $(ASSEMBLY_NAME) /package $(PACKAGE_SHARP) /gacdir $(prefix)/lib || exit 1;	\
	  fi;												\
	fi
