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
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("Microsoft.DirectX")
clr.AddReferenceByPartialName("Microsoft.DirectX.Direct3D")

import System
from System import Drawing
from System.Windows import Forms
from Microsoft import DirectX
from Microsoft.DirectX import Direct3D


class MeshRenderable(object):
    def __init__(self, device, name, file):
        PositionableObject.__init__(self)
        RotatableObject.__init__(self)
        
        self.Name = name
        materials = Direct3D.MaterialList()
        self.Mesh = Direct3D.Mesh(device, file,
                                  Direct3D.MeshFlags.SystemMemory,
                                  None, materials, None)        
        self.Materials = []
        self.Textures = []
        for i in range(materials.Count):
            # load material, set color
            material = materials[i].Material
            material.AmbientColor = material.DiffuseColor
            
            # load texture, if possible
            texture = None
            texFile = materials[i].TextureFileName
            if texFile is not None and texFile.Length:
                texture = Direct3D.Texture(device, texFile)
            
            # insert the material and texture into the lists
            self.Materials.append(material)
            self.Textures.append(texture)
    
    def GetWorldMatrix(self):
        return DirectX.Matrix.Identity
        

class RenderWindow(Forms.Form):
    def __init__(self, sceneManager):
        self.SceneManager = sceneManager
        self.Text = "IronPython Direct3D"
        self.ClientSize = Drawing.Size(640, 480)
        

    def OnKeyPress(self, args):
        if int(args.KeyChar) == int(System.Windows.Forms.Keys.Escape):
            self.Dispose()
            

    def OnResize(self, args):
        self.SceneManager.Paused = not self.Visible or \
            (self.WindowState == Forms.FormWindowState.Minimized)


class SceneManager(object):
    def __init__(self):
        self.Device = None
        self.Paused = False
        self.Background = System.Drawing.Color.Black
        self.Objects = {}
        
    def LoadMesh(self, name, filename):
        mesh = MeshRenderable(self.Device, name, filename)
        self.Objects[mesh.Name] = mesh
        return mesh
        

    def InitGraphics(self, handle):
        params = Direct3D.PresentParameters()
        params.Windowed = True
        params.SwapEffect = Direct3D.SwapEffect.Discard
        params.EnableAutoDepthStencil = True
        params.AutoDepthStencilFormat = Direct3D.DepthFormat.D16

        self.Device = Direct3D.Device(0, Direct3D.DeviceType.Hardware, handle,
                                      Direct3D.CreateFlags.SoftwareVertexProcessing,
                                      params)

        self.Device.RenderState.ZBufferEnable = True
        self.Device.Transform.Projection = DirectX.Matrix.PerspectiveFovLH(System.Math.PI/4.0, 1, 1, 100)
        self.Device.Transform.View = DirectX.Matrix.LookAtLH(DirectX.Vector3(0, 3, -5), DirectX.Vector3(0, 0, 0), DirectX.Vector3(0, 1, 0))
        self.Device.RenderState.Ambient = Drawing.Color.White

        # ensure we are not paused
        self.Paused = False

        return True

    def Render(self):
        if self.Device is None or self.Paused:
            return
        
        self.Device.Clear(Direct3D.ClearFlags.Target | Direct3D.ClearFlags.ZBuffer,
                          self.Background, 1, 0)
        self.Device.BeginScene()

        for mesh in self.Objects.Values:
            if hasattr(mesh, "GetWorldMatrix"):
                self.Device.Transform.World = mesh.GetWorldMatrix()
                
                materials = mesh.Materials
                textures = mesh.Textures
                
                for i in range(len(materials)):
                    self.Device.Material = materials[i]
                    if i < len(textures):
                        self.Device.SetTexture(0, textures[i])
                    else:
                        self.Device.SetTexture(0, None)
                    mesh.Mesh.DrawSubset(i)

        self.Device.EndScene()
        self.Device.Present()


    def Go(self, window):
        while window.Created:
            self.Render()
            Forms.Application.DoEvents()
        


def main():
    sceneManager = SceneManager()
    window = RenderWindow(sceneManager)

    if not sceneManager.InitGraphics(window.Handle):
        Forms.MessageBox.Show("Could not init Direct3D.")

    else:
        window.Show()
        sceneManager.Go(window)

if __name__ == '__main__':
    main()
