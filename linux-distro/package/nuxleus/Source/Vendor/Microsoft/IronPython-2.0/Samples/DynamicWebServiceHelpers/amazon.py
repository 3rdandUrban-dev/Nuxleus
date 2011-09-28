#####################################################################################
#
#  Copyright (c) Microsoft Corporation. All rights reserved.
#
# This source code is subject to terms and conditions of the Microsoft Public License. A 
# copy of the license can be found in the License.html file at the root of this distribution. If 
# you cannot locate the  Microsoft Public License, please send an email to 
# ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
# by the terms of the Microsoft Public License.
#
# You must not remove this notice, or any other, from this software.
#
#
#####################################################################################

# sample calling Amazon WSDL-based web service

# requires subscription id from amazon.com
from sys import argv
if len(argv)==1:
    print "This sample needs an Amazon.com subscription ID passed to it from the command line!"
    from sys import exit
    exit(1)
else:
    subscriptionId = argv[1]

searchString = 'Python Programming'

if subscriptionId is None:
    raise RuntimeError, 'Amazon subscription id is required to run this sample'

import clr
clr.AddReference("DynamicWebServiceHelpers.dll")
import DynamicWebServiceHelpers

print 'loading web service'
ws = DynamicWebServiceHelpers.WebService.Load('http://webservices.amazon.com/AWSECommerceService/AWSECommerceService.wsdl')

print 'composing data for web service request'
sr = ws.ServiceNamespace.ItemSearchRequest()
sr.Keywords = searchString
sr.SearchIndex = 'Books'
sr.ResponseGroup = tuple(['ItemAttributes'])
sr.ItemPage = '1'
s = ws.ServiceNamespace.ItemSearch()
s.SubscriptionId = subscriptionId
s.AssociateTag = ''
s.Request = tuple([ sr ])


print 'calling web service'
r = ws.ItemSearch(s)

print 'search for "%s": %s results (%s pages):' % (searchString, r.Items[0].TotalResults, r.Items[0].TotalPages)
page = 1
while r <> None and r.Items[0] <> None and r.Items[0].Item <> None:
    for book in r.Items[0].Item:
        print '   %s' % (book.ItemAttributes.Title)
    page = page+1
    sr.ItemPage = str(page)
    r = ws.ItemSearch(s)
