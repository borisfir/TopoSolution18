using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using CustomDialogs;

namespace CustomDialogs
{
  public class Commands : IExtensionApplication
  {
    static TypeViewerPalette tvp;

    public void Initialize()
    {
      tvp = new TypeViewerPalette();

      DocumentCollection dm =
        Application.DocumentManager;
      dm.DocumentCreated +=
        new DocumentCollectionEventHandler(OnDocumentCreated);
      foreach (Document doc in dm)
      {
        doc.Editor.PointMonitor +=
          new PointMonitorEventHandler(OnMonitorPoint);
      }
    }

    public void Terminate()
    {
      try
      {
        DocumentCollection dm =
          Application.DocumentManager;
        if (dm != null)
        {
          Editor ed = dm.MdiActiveDocument.Editor;
          ed.PointMonitor -=
            new PointMonitorEventHandler(OnMonitorPoint);
        }
      }
      catch (System.Exception)
      {
        // The editor may no longer
        // be available on unload
      }
    }

    private void OnDocumentCreated(
      object sender,
      DocumentCollectionEventArgs e
    )
    {
      e.Document.Editor.PointMonitor +=
        new PointMonitorEventHandler(OnMonitorPoint);
    }

    private void OnMonitorPoint(
      object sender,
      PointMonitorEventArgs e
    )
    {
      FullSubentityPath[] paths =
        e.Context.GetPickedEntities();
      if (paths.Length <= 0)
      {
        tvp.SetObjectId(ObjectId.Null);
        return;
      };

      ObjectIdCollection idc = new ObjectIdCollection();
      foreach (FullSubentityPath path in paths)
      {
        // Just add the first ID in the list from each path
        ObjectId[] ids = path.GetObjectIds();
        idc.Add(ids[0]);
      }
      tvp.SetObjectIds(idc);
    }

    [CommandMethod("vt",CommandFlags.UsePickSet)]
    public void ViewType()
    {
      tvp.Show();
    }
  }
}