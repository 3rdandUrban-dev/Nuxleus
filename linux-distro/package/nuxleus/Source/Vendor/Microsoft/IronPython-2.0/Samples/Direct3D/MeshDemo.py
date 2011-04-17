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

class SceneCreator(object):
    def OnSceneCreate(self, sm):
        self.Tiger = sm.LoadMesh("Tiger", "tiger.x")
        
        cam = sm.CreateCamera("Player Cam")
        cam.Position = [0, 3, -7]
        
        cam.LookAt(self.Tiger.Position)
        sm.AddListener(TigerAnimator())
        return True
                

class TigerAnimator(object):
    def OnSceneBegin(self, sceneManager):
        self.Tiger = sceneManager.Objects["Tiger"]
        return True
        
    def OnFrame(self, elapsed):
        self.Tiger.Yaw(elapsed)

if __name__ == '__main__':
    Root().Main([SceneCreator])
