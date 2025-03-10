using Autodesk.DataExchange.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PrimitiveData = Autodesk.GeometryPrimitives.Data;
using Autodesk.GeometryPrimitives.Data.DX;
using Autodesk.GeometryUtilities.MeshAPI;

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

        public Element CreateMeshThroughMeshApi(ElementDataModel elementDataModel)
        {
            var complexMeshObject = GetMeshObject();
            var complexMeshGeom = ElementDataModel.CreateMeshGeometry(new GeometryProperties(complexMeshObject, "Complex Mesh With Color"));
            var complexMeshElement = elementDataModel.AddElement(new ElementProperties("ComplexMesh", "ComplexSampleMesh", "Mesh", "Mesh", "Complex In memory mesh"));
            elementDataModel.SetElementGeometryByElement(complexMeshElement, new List<ElementGeometry> { complexMeshGeom });
            return complexMeshElement;
        }

        private Autodesk.GeometryUtilities.MeshAPI.Mesh GetMeshObject()
        {
            return new Autodesk.GeometryUtilities.MeshAPI.Mesh()
            {
                MeshColor = new Color(0.5f, 0.5f, 0.5f, 1.0f),  // mesh body color
                Vertices = new List<Vertex>
                {
                    new Vertex(0.0, 0.0, 0.0),
                    new Vertex(1.0, 0.0, 0.0),
                    new Vertex(0.0, 1.0, 0.0),
                    new Vertex(1.0, 1.0, 0.0),
                    new Vertex(0.0, 0.0, 1.0),
                    new Vertex(1.0, 0.0, 1.0),
                    new Vertex(0.0, 1.0, 1.0),
                    new Vertex(1.0, 1.0, 1.0),
                    new Vertex(0.5, 0.5, 1.5),
                },
                Faces = new List<Face>
                {
                    new Face()
                    {
                        Corners = new List<int> { 0, 1, 2 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                        },
                        FaceColor = new Color(0.2f, 0.2f, 0.9f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 2, 1, 3 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                        },
                        FaceColor = new Color(0.2f, 0.9f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 0, 1, 4 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 1, 0),
                            new Normal(0, 1, 0),
                            new Normal(0, 1, 0),
                        },
                        FaceColor = new Color(0.9f, 0.2f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 1, 5, 4 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 1, 0),
                            new Normal(0, 1, 0),
                            new Normal(0, 1, 0),
                        },
                        FaceColor = new Color(0.9f, 0.2f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 0, 2, 4 },
                        Normals = new List<Normal>
                        {
                            new Normal(1, 0, 0),
                            new Normal(1, 0, 0),
                            new Normal(1, 0, 0),
                        },
                        FaceColor = new Color(0.2f, 0.9f, 0.9f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 2, 6, 4 },
                        Normals = new List<Normal>
                        {
                            new Normal(1, 0, 0),
                            new Normal(1, 0, 0),
                            new Normal(1, 0, 0),
                        },
                        FaceColor = new Color(0.2f, 0.9f, 0.9f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 1, 3, 5 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, -1, 0),
                            new Normal(0, -1, 0),
                            new Normal(0, -1, 0),
                        },
                        FaceColor = new Color(0.9f, 0.9f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 3, 7, 5 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, -1, 0),
                            new Normal(0, -1, 0),
                            new Normal(0, -1, 0),
                        },
                        FaceColor = new Color(0.9f, 0.9f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 2, 3, 6 },
                        Normals = new List<Normal>
                        {
                            new Normal(-1, 0, 0),
                            new Normal(-1, 0, 0),
                            new Normal(-1, 0, 0),
                        },
                        FaceColor = new Color(0.9f, 0.2f, 0.9f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 3, 7, 6 },
                        Normals = new List<Normal>
                        {
                            new Normal(-1, 0, 0),
                            new Normal(-1, 0, 0),
                            new Normal(-1, 0, 0),
                        },
                        FaceColor = new Color(0.9f, 0.2f, 0.9f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 4, 5, 6 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, -1),
                            new Normal(0, 0, -1),
                            new Normal(0, 0, -1),
                        },
                        FaceColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 5, 7, 6 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, -1),
                            new Normal(0, 0, -1),
                            new Normal(0, 0, -1),
                        },
                        FaceColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 4, 6, 8 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                        },
                        FaceColor = new Color(0.5f, 0.5f, 0.5f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 5, 7, 8 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                        },
                        FaceColor = new Color(0.5f, 0.5f, 0.5f, 1.0f),  // face color
                    },
                    new Face()
                    {
                        Corners = new List<int> { 6, 7, 8 },
                        Normals = new List<Normal>
                        {
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                            new Normal(0, 0, 1),
                        },
                        FaceColor = new Color(0.5f, 0.5f, 0.5f, 1.0f),  // face color
                    },
                },
            };
        }

        public Element CreateIfc(ElementDataModel elementDataModel)
        {
            var details = ifcFileDetails[random.Next(ifcFileDetails.Count)];
            return CreateGeometry(elementDataModel, details);
        }

        private Element CreateGeometry(ElementDataModel elementDataModel, Tuple<string, string, string, string> geometryDetails)
        {
            var path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Assets\\" + geometryDetails.Item1;
            var geometry = ElementDataModel.CreateGeometry(new GeometryProperties(path, commonRenderStyle));
            var element = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SampleGeometry", geometryDetails.Item2, geometryDetails.Item3, geometryDetails.Item4));
            var elementGeometry = new List<ElementGeometry> { geometry };
            elementDataModel.SetElementGeometryByElement(element, elementGeometry);
            return element;
        }

        public Element AddLine(ElementDataModel elementDataModel)
        {
            var newElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SampleLine", "Line", "Line", "Generic Line"));

            var newBRepElementGeometry = new List<ElementGeometry>();

            CurveSet setOfLines = new CurveSet();

            PrimitiveData.Line lineone = new PrimitiveData.Line(new
                PrimitiveData.Point3d
            { X = random.Next(999), Y = random.Next(999), Z = random.Next(999) },
                new PrimitiveData.Vector3d { X = random.Next(999), Y = random.Next(999), Z = random.Next(999) });
            lineone.Range = new PrimitiveData.ParamRange()
            {
                High = 3.5,
                Low = 0,
                Type = PrimitiveData.ParamRange.RangeType.Finite
            };

            setOfLines.Add(lineone);

            newBRepElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(new GeometryProperties(setOfLines, commonRenderStyle)));
            elementDataModel.SetElementGeometryByElement(newElement, newBRepElementGeometry);
            return newElement;
        }

        public Element AddPoint(ElementDataModel elementDataModel)
        {
            var geomContainer = new GeometryContainer();
            var newPointElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SamplePoint", "Point", "Point", "Point"));
            var newPointElementGeometry = new List<ElementGeometry>();
            DesignPoint point = new DesignPoint(random.Next(999), random.Next(999), random.Next(999));
            newPointElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(new GeometryProperties(point, commonRenderStyle)));
            elementDataModel.SetElementGeometryByElement(newPointElement, newPointElementGeometry);
            return newPointElement;
        }

        public Element AddCircle(ElementDataModel elementDataModel)
        {
            var geomContainer = new GeometryContainer();
            var circleElement = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SampleCircle", "Circle", "Circle", "Circle"));
            var newPointElementGeometry = new List<ElementGeometry>();
            var center = new PrimitiveData.Point3d(random.Next(999), random.Next(999), random.Next(999));
            var normal = new PrimitiveData.Vector3d(0, 0, 1);
            var radius = new PrimitiveData.Vector3d(random.Next(50), 0, 0);
            var circle = new PrimitiveData.Circle(center, normal, radius);
            geomContainer.Curves.Add(circle);
            newPointElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(new GeometryProperties(geomContainer, commonRenderStyle)));
            elementDataModel.SetElementGeometryByElement(circleElement, newPointElementGeometry);
            return circleElement;
        }

        public Element AddPrimitive(ElementDataModel elementDataModel)
        {
            //....Primitive geometry - Circle...
            var primitive = elementDataModel.AddElement(new ElementProperties(Guid.NewGuid().ToString(), "SamplePrimitive", "Primitive", "Primitive", "Primitive"));
            var circleElementGeometry = new List<ElementGeometry>();
            var geomContainer = new GeometryContainer()
            {
                Curves = new List<PrimitiveData.Curve>()
                        {
                             new PrimitiveData.Line()
                             {
                                Position = new PrimitiveData.Point3d(0, 0, 0),
                                Direction = new PrimitiveData.Vector3d(1, 0, 0),
                                Range = new PrimitiveData.ParamRange(PrimitiveData.ParamRange.RangeType.Finite, -1000, 1000)
                             },
                            new PrimitiveData.Circle()
                            {
                                Center = new PrimitiveData.Point3d(0, 0, 0),
                                Normal = new PrimitiveData.Vector3d(0, 0, 1),
                                Radius = new PrimitiveData.Vector3d(500, 0, 0)
                            },
                            //CCW 90degree
                            new PrimitiveData.Circle()
                            {
                                Center = new PrimitiveData.Point3d(700, 0, 0),
                                Normal = new PrimitiveData.Vector3d(0, 0, 1),
                                Radius = new PrimitiveData.Vector3d(200, 0, 0),
                                Range = new PrimitiveData.ParamRange(PrimitiveData.ParamRange.RangeType.Finite,0,1.5708)
                            },
                            //CW 90degree
                            new PrimitiveData.Circle()
                            {
                                Center = new PrimitiveData.Point3d(1000, 0, 0),
                                Normal = new PrimitiveData.Vector3d(0, 0, 1),
                                Radius = new PrimitiveData.Vector3d(200, 0, 0),
                                Range = new PrimitiveData.ParamRange(PrimitiveData.ParamRange.RangeType.Finite,-1.5708,0)
                            },
                            new PrimitiveData.BCurve()
                            {
                                Degree = 3,
                                Knots = new List<double>() {
                                    0, 0, 0, 0,
                                    22.052499319464456,
                                    39.56011633518649,
                                    61.767382623682536,
                                    86.37111048613733, 86.37111048613733, 86.37111048613733, 86.37111048613733
                                },
                                ControlPoints = new List<PrimitiveData.Point3d>()
                                {
                                    new PrimitiveData.Point3d(-2117.6323100866352, -578.0819231498238, 0),
                                    new PrimitiveData.Point3d(-1412.0457600104123, -249.06151135811626, 0),
                                    new PrimitiveData.Point3d(-1846.6021471151125, 185.49487574658576, 0),
                                    new PrimitiveData.Point3d(-1223.20576487363, 185.49487574658374, 0),
                                    new PrimitiveData.Point3d(-908.8865372007543, 366.96716645499356, 0),
                                    new PrimitiveData.Point3d(-981.7326274133812, -674.7804577820922, 0),
                                    new PrimitiveData.Point3d(-218.14218928271805, -1030.8485267763535, 0)
                                },
                                Weights = new List<double>() { 1, 1, 1, 1, 1, 1, 1 }
                            },
                            new PrimitiveData.Ellipse()
                            {
                                Center = new PrimitiveData.Point3d(0, 0, 0),
                                Normal = new PrimitiveData.Vector3d(0, 0, 1),
                                MajorRadius = new PrimitiveData.Vector3d(500, 0, 0),
                                RadiusRatio = 0.7
                            }

                        },
                Surfaces = new List<PrimitiveData.Surface>()
                        {
                             new PrimitiveData.Plane()
                             {
                                 Origin = new PrimitiveData.Point3d(0, -270.888, 7.8900e-3),
                                 Normal = new PrimitiveData.Vector3d(1, 0, 0),
                                 UAxis = new PrimitiveData.Vector3d(0, 1, 0),
                                 URange = new PrimitiveData.ParamRange(
                                     PrimitiveData.ParamRange.RangeType.Finite,
                                     0,
                                     2000
                                 ),
                                 VRange = new PrimitiveData.ParamRange(
                                     PrimitiveData.ParamRange.RangeType.Finite,
                                     0,
                                     1000
                                 )
                             },
                             new PrimitiveData.Plane()
                             {
                                 Origin = new PrimitiveData.Point3d(0, -2000, 0),
                                 Normal = new PrimitiveData.Vector3d(1, 0, 0),
                                 UAxis = new PrimitiveData.Vector3d(0, 1, 0),
                                 URange = new PrimitiveData.ParamRange(
                                     PrimitiveData.ParamRange.RangeType.Finite,
                                     0,
                                     700
                                 ),
                             }
                        },
            };

            circleElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(new GeometryProperties(geomContainer, commonRenderStyle)));
            elementDataModel.SetElementGeometryByElement(primitive, circleElementGeometry);
            return primitive;
        }

        public Element AddPolyline(ElementDataModel dataModel)
        {
            var polyLineElement = dataModel.AddElement(new ElementProperties("Polyline", "SamplePolyline", "PolylineGenerics", "PolylineGeneric", "PolylineElement"));
            var polyLineElementGeometry = new List<ElementGeometry>();
            var geomContainer = new GeometryContainer()
            {
                Curves = new List<PrimitiveData.Curve>()
                {
                    new PrimitiveData.Polyline()
                    {
                        Range = new PrimitiveData.ParamRange(PrimitiveData.ParamRange.RangeType.Finite, 0.0, 2.0),
                        Closed = false,
                        Points = new List<PrimitiveData.Point3d>()
                        {
                            new PrimitiveData.Point3d(12.5, 4, 0),
                            new PrimitiveData.Point3d(4.5, 4, 0),
                            new PrimitiveData.Point3d(11.25, 0, 0)
                        }
                    }
                }
            };

            polyLineElementGeometry.Add(ElementDataModel.CreatePrimitiveGeometry(new GeometryProperties(geomContainer, commonRenderStyle)));
            dataModel.SetElementGeometryByElement(polyLineElement, polyLineElementGeometry);
            return polyLineElement;
        }
    }
}