//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------


//------------------------------------------------------------------------------
// Hard coded images referenced from C++ code
//------------------------------------------------------------------------------

//   editor/SelectHandle.png
//   editor/DefaultHandle.png
//   editor/LockedHandle.png


//------------------------------------------------------------------------------
// Functions
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Mission Editor 
//------------------------------------------------------------------------------

function Editor::create()
{
   // Not much to do here, build it and they will come...
   // Only one thing... the editor is a gui control which
   // expect the Canvas to exist, so it must be constructed
   // before the editor.
   new EditManager(Editor)
   {
      profile = "GuiContentProfile";
      horizSizing = "right";
      vertSizing = "top";
      position = "0 0";
      extent = "640 480";
      minExtent = "8 8";
      visible = "1";
      setFirstResponder = "0";
      modal = "1";
      helpTag = "0";
      open = false;
   };
}

function Editor::getUndoManager(%this)
{
   if ( !isObject( %this.undoManager ) )
   {
      /// This is the global undo manager used by all
      /// of the mission editor sub-editors.
      %this.undoManager = new UndoManager( EUndoManager )
      {
         numLevels = 200;
      };
   }
   return %this.undoManager;
}

function Editor::setUndoManager(%this, %undoMgr)
{
   %this.undoManager = %undoMgr;
}

function Editor::onAdd(%this)
{
   // Ignore Replicated fxStatic Instances.
   EWorldEditor.ignoreObjClass("fxShapeReplicatedStatic");
}

function Editor::checkActiveLoadDone()
{
   if(isObject(EditorGui) && EditorGui.loadingMission)
   {
      Canvas.setContent(EditorGui);
      EditorGui.loadingMission = false;
      return true;
   }
   return false;
}

//------------------------------------------------------------------------------
function toggleEditor(%make)
{
   if (Canvas.isFullscreen())
   {
      MessageBoxOK("Windowed Mode Required", "Please switch to windowed mode to access the Mission Editor.");
      return;
   }
   
   if (%make)
   {      
      %timerId = startPrecisionTimer();
      
      if( $InGuiEditor )
         GuiEdit();
         
      pushInstantGroup();
      
      if ( !isObject( Editor ) )
      {
         Editor::create();
         MissionCleanup.add( Editor );
         MissionCleanup.add( Editor.getUndoManager() );
      }
      
      if( EditorIsActive() )
      {
         if (theLevelInfo.type $= "DemoScene") 
         {
            commandToServer('dropPlayerAtCamera');
            Editor.close("SceneGui");   
         } 
         else 
         {
            Editor.close("PlayGui");
         }
      }
      else 
      {
         if ( !$GuiEditorBtnPressed )
         {
            canvas.pushDialog( EditorLoadingGui );
            canvas.repaint();
         }
         else
         {
            $GuiEditorBtnPressed = false;
         }

         Editor.open();

         canvas.popDialog(EditorLoadingGui);
      }
      
      popInstantGroup();
      
      %elapsed = stopPrecisionTimer( %timerId );
      warn( "Time spent in toggleEditor() : " @ %elapsed / 1000.0 @ " s" );
   }
}
