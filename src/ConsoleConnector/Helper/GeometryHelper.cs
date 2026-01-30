using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;
using Autodesk.GeometryPrimitives.Data;
using Autodesk.GeometryPrimitives.Data.DX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class GeometryHelper
    {
        private List<Tuple<string, string, string, string>> ifcFileDetails = new List<Tuple<string, string, string, string>>
        {
            new Tuple<string, string, string, string>("BasinAdvancedBrep.ifc","Generic IFC","Generic IFC","Generic IFC"),
            new Tuple<string, string, string, string>("Beam.ifc","Beam IFC","Beam IFC","Beam IFC"),
        };
        private List<Tuple<string, string, string, string>> meshFileDetails = new List<Tuple<string, string, string, string>>
        {
            new Tuple<string, string, string, string>("mesh1.obj","Mesh","Mesh","Mesh"),
            new Tuple<string, string, string, string>("mesh2.obj","Mesh","Mesh","Mesh"),
        };
        private List<Tuple<string, string, string, string>> brepFileDetails = new List<Tuple<string, string, string, string>>
        {
            new Tuple<string, string, string, string>("11DB159F6864D8FC02B33D7E9280498F08DFC4FB.stp","Walls","Wall","Generic Wall"),
            new Tuple<string, string, string, string>("1000mm_rod.stp","Rod","Rod","Generic Rod"),
            new Tuple<string, string, string, string>("box.stp","Box","Box","Generic Box"),
            new Tuple<string, string, string, string>("cone.stp","Cone","Cone","Generic Cone"),
            new Tuple<string, string, string, string>("cylinder.stp","Cylinder","Cylinder","Generic Cylinder"),
            new Tuple<string, string, string, string>("sphere.stp","Sphere","Sphere","Generic Sphere"),
            new Tuple<string, string, string, string>("fcdc7083-ed51-31d1-9d0e-c608b400ede8.stp","Generic","Generic","Generic"),
            new Tuple<string, string, string, string>("nist_ftc_09_asme1_rd.stp","Generic","Generic","Generic"),
        };
        Random random = new Random();
        RenderStyle commonRenderStyle = new RenderStyle()
        {
            Name = "Door Render Style",
            RGBA = new RGBA()
            {
                Red = 255,
                Blue = 0,
                Green = 0,
                Alpha = 255
            },
            Transparency = 1
        };

        public Element CreateBrep(ElementDataModel elementDataModel)
        {
            var details = brepFileDetails[random.Next(brepFileDetails.Count)];
            return CreateGeometry(elementDataModel, details);
        }

        public Element CreateMesh(ElementDataModel elementDataModel)
        {
            var details = meshFileDetails[random.Next(meshFileDetails.Count)];
            return CreateGeometry(elementDataModel, details);
        }

        public Element CreateIfc(ElementDataModel elementDataModel)
        {
            var details = ifcFileDetails[random.Next(ifcFileDetails.Count)];
            return CreateGeometry(elementDataModel, details);
        }

        private Element CreateGeometry(ElementDataModel elementDataModel, Tuple<string, string, string, string> geometryDetails)
        {
            var path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Assets\\" + geometryDetails.Item1;
            var format = GetGeometryFormat(geometryDetails.Item1);
            var geometry = ElementDataModel.CreateFileGeometry(path, format, commonRenderStyle);
            var element = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(),"SampleGeometry", geometryDetails.Item2, geometryDetails.Item3, geometryDetails.Item4));
            var elementGeometry = new List<ElementGeometry> { geometry };
            elementDataModel.SetElementGeometry(element, elementGeometry);
            return element;
        }

        private GeometryFormat GetGeometryFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".stp":
                case ".step":
                    return GeometryFormat.Step;
                case ".ifc":
                    return GeometryFormat.Ifc;
                case ".obj":
                    return GeometryFormat.Obj;
                default:
                    return GeometryFormat.Unknown;
            }
        }

        public Element AddLine(ElementDataModel elementDataModel)
        {
            var newElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SampleLine", "Line", "Line", "Generic Line"));

            var newBRepElementGeometry = new List<ElementGeometry>();

            var geomContainer = new GeometryContainer();

            Line lineone = new Line(new 
                Point3d { X = random.Next(999), Y = random.Next(999), Z = random.Next(999) }, 
                new Vector3d { X = random.Next(999), Y = random.Next(999), Z = random.Next(999) });
            lineone.Range = new ParamRange()
            {
                High = 3.5,
                Low = 0,
                Type = ParamRange.RangeType.Finite
            };

            geomContainer.Curves.Add(lineone);

            newBRepElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(geomContainer, commonRenderStyle));
            elementDataModel.SetElementGeometry(newElement, newBRepElementGeometry);
            return newElement;
        }

        public Element AddPoint(ElementDataModel elementDataModel)
        {
            var newPointElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(),"SamplePoint", "Point", "Point", "Point"));
            var newPointElementGeometry = new List<ElementGeometry>();
            DesignPoint point = new DesignPoint(random.Next(999), random.Next(999), random.Next(999));
            newPointElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(point, commonRenderStyle));
            elementDataModel.SetElementGeometry(newPointElement, newPointElementGeometry);
            return newPointElement;
        }

        public Element AddCircle(ElementDataModel elementDataModel)
        {
            var geomContainer = new GeometryContainer();
            var circleElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(),"SampleCircle", "Circle", "Circle", "Circle"));
            var newPointElementGeometry = new List<ElementGeometry>();
            var center = new Point3d(random.Next(999), random.Next(999), random.Next(999));
            var normal = new Vector3d(0, 0, 1);
            var radius = new Vector3d(random.Next(50), 0, 0);
            var circle = new Circle(center, normal, radius);
            geomContainer.Curves.Add(circle);
            newPointElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(geomContainer, commonRenderStyle));
            elementDataModel.SetElementGeometry(circleElement, newPointElementGeometry);
            return circleElement;
        }

        public Element AddPrimitive(ElementDataModel elementDataModel)
        {
            //....Primitive geometry - Circle...
            var primitive = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(),"SamplePrimitive", "Primitive", "Primitive", "Primitive"));
            var circleElementGeometry = new List<ElementGeometry>();
            var geomContainer = new GeometryContainer()
            {
                Curves = new List<Curve>
                        {
                             new Line()
                             {
                                Position = new Point3d(0, 0, 0),
                                Direction = new Vector3d(1, 0, 0),
                                Range = new ParamRange(ParamRange.RangeType.Finite, -1000, 1000)
                             },
                            new Circle()
                            {
                                Center = new Point3d(0, 0, 0),
                                Normal = new Vector3d(0, 0, 1),
                                Radius = new Vector3d(500, 0, 0)
                            },
                            //CCW 90degree
                            new Circle()
                            {
                                Center = new Point3d(700, 0, 0),
                                Normal = new Vector3d(0, 0, 1),
                                Radius = new Vector3d(200, 0, 0),
                                Range = new ParamRange(ParamRange.RangeType.Finite,0,1.5708)
                            },
                            //CW 90degree
                            new Circle()
                            {
                                Center = new Point3d(1000, 0, 0),
                                Normal = new Vector3d(0, 0, 1),
                                Radius = new Vector3d(200, 0, 0),
                                Range = new ParamRange(ParamRange.RangeType.Finite,-1.5708,0)
                            },
                            new BCurve()
                            {
                                Degree = 3,
                                Knots = new List<double>() {
                                    0, 0, 0, 0,
                                    22.052499319464456,
                                    39.56011633518649,
                                    61.767382623682536,
                                    86.37111048613733, 86.37111048613733, 86.37111048613733, 86.37111048613733
                                },
                                ControlPoints = new List<Point3d>()
                                {
                                    new Point3d(-2117.6323100866352, -578.0819231498238, 0),
                                    new Point3d(-1412.0457600104123, -249.06151135811626, 0),
                                    new Point3d(-1846.6021471151125, 185.49487574658576, 0),
                                    new Point3d(-1223.20576487363, 185.49487574658374, 0),
                                    new Point3d(-908.8865372007543, 366.96716645499356, 0),
                                    new Point3d(-981.7326274133812, -674.7804577820922, 0),
                                    new Point3d(-218.14218928271805, -1030.8485267763535, 0)
                                },
                                Weights = new List<double>() { 1, 1, 1, 1, 1, 1, 1 }
                            },
                            new Ellipse()
                            {
                                Center = new Point3d(0, 0, 0),
                                Normal = new Vector3d(0, 0, 1),
                                MajorRadius = new Vector3d(500, 0, 0),
                                RadiusRatio = 0.7
                            }

                        },
                Surfaces = new List<Surface>
                        {
                             new Plane()
                             {
                                 Origin = new Point3d(0, -270.888, 7.8900e-3),
                                 Normal = new Vector3d(1, 0, 0),
                                 UAxis = new Vector3d(0, 1, 0),
                                 URange = new ParamRange(
                                     ParamRange.RangeType.Finite,
                                     0,
                                     2000
                                 ),
                                 VRange = new ParamRange(
                                     ParamRange.RangeType.Finite,
                                     0,
                                     1000
                                 )
                             },
                             new Plane()
                             {
                                 Origin = new Point3d(0, -2000, 0),
                                 Normal = new Vector3d(1, 0, 0),
                                 UAxis = new Vector3d(0, 1, 0),
                                 URange = new ParamRange(
                                     ParamRange.RangeType.Finite,
                                     0,
                                     700
                                 ),
                             }
                        },
            };

            circleElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(geomContainer, commonRenderStyle));
            elementDataModel.SetElementGeometry(primitive, circleElementGeometry);
            return primitive;
        }

        public Element AddPolyline(ElementDataModel dataModel)
        {
            var polyLineElement = dataModel.AddElement(new ElementProperties("Polyline","SamplePolyline", "PolylineGenerics", "PolylineGeneric", "PolylineElement"));
            var polyLineElementGeometry = new List<ElementGeometry>();
            var geomContainer = new GeometryContainer()
            {
                Curves = new List<Curve>
                {
                    new Polyline()
                    {
                        Range = new ParamRange(ParamRange.RangeType.Finite, 0.0, 2.0),
                        Closed = false,
                        Points = new List<Point3d>()
                        {
                            new Point3d(12.5, 4, 0),
                            new Point3d(4.5, 4, 0),
                            new Point3d(11.25, 0, 0)
                        }
                    }
                }
            };

            polyLineElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(geomContainer, commonRenderStyle));
            dataModel.SetElementGeometry(polyLineElement, polyLineElementGeometry);
            return polyLineElement;
        }
    }
}
