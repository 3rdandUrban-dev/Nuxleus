#####################################################################################
#
# Copyright (c) Microsoft Corporation. 
#
# This source code is subject to terms and conditions of the Microsoft Public
# License. A  copy of the license can be found in the License.html file at the
# root of this distribution. If  you cannot locate the  Microsoft Public
# License, please send an email to  dlr@microsoft.com. By using this source
# code in any fashion, you are agreeing to be bound by the terms of the 
# Microsoft Public License.
#
# You must not remove this notice, or any other, from this software.
#
#####################################################################################

from math import *
def vecadd(v1,v2):
       return [x+y for x,y in zip(v1,v2)]

def vecsub(v1, v2):
       return [x-y for x,y in zip(v1,v2)]

def scalarprod(s, v1):
       return [x * s for x in v1]

def dotprod(v1, v2):
       t = [x*y for x,y in zip(v1,v2)]
       s = 0.0
       for x in t:
           s += x
       return s
    
def length(vector):
       return sqrt(dotprod(vector,vector))

def vector(point1, point2):
       return [y - x for x,y in zip(point1, point2)]
        
def dist(point1, point2):
       return length(vector(point1, point2))

def normalize(vector):
       l = length(vector)
       return scalarprod(1.0/l, vector)


def KochFunc(point1, point2):
       if (len(point1) != 2):
              return []
       v = vector(point1,point2)
       n3 = scalarprod(1.0/3.0, v)
       c = cos(-pi/3)
       s = sin(-pi/3)
       n3rotated = [n3[0] * c - n3[1] * s, n3[0]* s + n3[1] * c]
       p1 = point1
       p2 = vecadd(point1, n3)
       p3 = vecadd(p2, n3rotated)
       p4 = vecadd(p2,n3)
       p5 = point2
       return [p1, p2, p3, p4, p5]
        
def KochFuncOnSequence(s):
       retval = []
       for i in range(len(s)-1):
              retval += KochFunc(s[i], s[i+1])[:4]
       retval += KochFunc(s[len(s)-1], s[0])
       return retval

# and here's the second:

import clr
clr.AddReferenceByPartialName("PresentationCore")
clr.AddReferenceByPartialName("PresentationFramework")
clr.AddReferenceByPartialName("WindowsBase")

from math import *
from System.Windows import *
from System.Windows.Media import *
from System.Windows.Controls import *
from System.Windows.Shapes import *

def makePoints(sequence):
    retval = []
    for x in sequence:
        retval.append(Point(x[0],x[1]))
    return retval
 
def makeFigure(sequence):
    points = makePoints(sequence)
    figure = PathFigure()
    figure.StartPoint = points[0]
    s = PolyLineSegment()
    figure.Segments.Add(s)
    for x in points:
        s.Points.Add(x)
    return figure
 
def makeGeometry(sequence):
    g = PathGeometry()
    g.Figures.Add(makeFigure(sequence))
    return g
 
def makeSnowFlake(sequence):
    g = makeGeometry(sequence)
    p = Path()
    p.Data = g
    return p

Scale = 800.0
StartTriangle = [[0,100], [Scale,100], [Scale/2, Scale*sin(pi/3)+100]]
FirstDegreeFlake = KochFuncOnSequence(StartTriangle)
SecondDegreeFlake = KochFuncOnSequence(FirstDegreeFlake)

def drawFlake(target, degree = 2, brush=Brushes.Cyan):
    s = StartTriangle
    for i in range(degree):
        s = KochFuncOnSequence(s)
    p = makeSnowFlake(s)
    p.Fill = brush
    p.SetValue(Canvas.TopProperty, 50.0)
    target.Children.Add(p)
    return p

def sampleFlake(target):
     stop1 = GradientStop(Colors.Yellow, 0.0)
     stop2 = GradientStop(Colors.Orange, 1.0)
     stops = GradientStopCollection()
     stops.Add(stop1)
     stops.Add(stop2)
     p = drawFlake(target, 6, LinearGradientBrush(stops))
     p.SetValue(Canvas.TopProperty, 120.0)
     p.Stroke = Brushes.Violet
     return p

if globals().has_key('window'):
    sampleFlake(window.Content)
else:
    MessageBox.Show("Please set 'window' variable'")
