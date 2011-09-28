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

import clr
from framework import *

class PositionalMover(object):
    def __init__(self, obj, destination, speed):
        self.Object = obj
        self.Dest = Vectorize(destination)
        self.Speed = speed
        
    def OnFrame(self, elapsed):
        toTravel = self.Speed * elapsed
        direction = self.Dest - self.Object.Position
        distance = direction.Length()
        direction.Normalize()
        
        if distance < toTravel:
            self.Object.Translate(direction * single(distance))
            return True
        else:
            self.Object.Translate(direction * single(toTravel))


class PointMover(object):
    def __init__(self, obj):
        self.Object = obj
        self.Points = []
        self.Current = None
        
    def AddPoint(self, point, speed):
        self.Points.append((point, speed))
        
    def NextPoint(self):
        if len(self.Points):
            dest, speed = self.Points[0]
            del self.Points[0]
            self.Current = PositionalMover(self.Object, dest, speed)
            return True
        
    def OnFrame(self, elapsed):
        if self.Current is None:
            if not self.NextPoint():
                return True
            
        if self.Current.OnFrame(elapsed):
            return not self.NextPoint()

class DemoSceneCreator(object):
    def OnSceneCreate(self, sm):
        cam = sm.CreateCamera("Player Cam")
        cam.Position = [0, 3, -5]
        
        box = sm.LoadBasicObject("box", "box 1", Drawing.Color.Red, 0.25, 0.25, 0.25)
        box.Position = [-1, 0, 0]
        
        box = sm.LoadBasicObject("box", "box 2", Drawing.Color.Green, 0.25, 0.25, 0.25)
        box.Position = [0, 0, 0]
        cam.LookAt(box.Position)
        
        box = sm.LoadBasicObject("box", "box 3", Drawing.Color.Blue, 0.25, 0.25, 0.25)
        box.Position = [1, 0, 0]

        return True

    def OnSceneBegin(self, sm):
        pm = PointMover(sm.Objects["box 1"])
        pm.AddPoint( [2, -1, 0], 1 )
        pm.AddPoint( [0, 1, 2], 1 )
        pm.AddPoint( [0, 0, 10], 1 )
        sm.AddListener(pm) 
        
        pm = PointMover(sm.Objects["box 2"])
        pm.AddPoint( [2, 1, 0], 1 )
        pm.AddPoint( [-1, -3, 5], 1 )
        pm.AddPoint( [-1, 0, 10], 1 )
        sm.AddListener(pm) 
        
        pm = PointMover(sm.Objects["box 3"])
        pm.AddPoint( [-2, -1, 0], 1 )
        pm.AddPoint( [1, -3, 0], 1 )
        pm.AddPoint( [1, 0, 10], 1.15 )
        sm.AddListener(pm)

        return True

if __name__ == '__main__':
    Root().Main([DemoSceneCreator])
