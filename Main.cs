using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Module_6_3
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var level1 = LevelUtils.GetLevel(commandData, "Уровень 1");
            var level2 = LevelUtils.GetLevel(commandData, "Уровень 2");



            List<Wall> walls = WallUtils.CreateWalls(commandData, level1, level2);
            AddDoor(doc,level1, walls[0]);
            AddWindow(doc, level1, walls[1]);
            AddWindow(doc, level1, walls[2]);
           AddWindow(doc, level1, walls[3]);
            return Result.Succeeded;
        }

        private void AddWindow(Document doc, Level level1, Wall wall)
        {
            using (Transaction tr = new Transaction(doc, "Create window"))
            {
                tr.Start();

                FamilySymbol windowType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 1830 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();


                
                LocationCurve hostCurve = wall.Location as LocationCurve;
                XYZ point1 = hostCurve.Curve.GetEndPoint(0);
                XYZ point2 = hostCurve.Curve.GetEndPoint(1);
                XYZ centerPoint = (point1 + point2) / 2;

                double wallHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                XYZ centerHeightPoint = new XYZ(centerPoint.X, centerPoint.Y, level1.Elevation + wallHeight / 3);


                if (!windowType.IsActive)
                    windowType.Activate();

                doc.Create.NewFamilyInstance(centerHeightPoint, windowType, wall, level1, StructuralType.NonStructural);

                tr.Commit();
            }

        }


        private void AddDoor(Document doc, Level level1, Wall wall)
        {
            using (Transaction tr = new Transaction(doc, "Create door"))
            {
                tr.Start();

                FamilySymbol doorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 2134 мм"))
                .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
                .FirstOrDefault();

                LocationCurve hostCurve = wall.Location as LocationCurve;
                XYZ point1 = hostCurve.Curve.GetEndPoint(0);
                XYZ point2 = hostCurve.Curve.GetEndPoint(1);
                XYZ point = (point1 + point2) / 2;

                if (!doorType.IsActive)
                    doorType.Activate();

                doc.Create.NewFamilyInstance(point, doorType, wall, level1, StructuralType.NonStructural);

                tr.Commit();
            }
            
        }
    }
}
