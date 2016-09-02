using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Circulation_Toolkit
{
    public class ToolkitInfo : GH_AssemblyInfo
  {
    public override string Name
    {
        get
        {
            return "Circulation";
        }
    }
    public override Bitmap Icon
    {
        get
        {
            //Return a 24x24 pixel bitmap to represent this GHA library.
            return null;
        }
    }
    public override string Description
    {
        get
        {
            //Return a short string describing the purpose of this GHA library.
            return "";
        }
    }
    public override Guid Id
    {
        get
        {
            return new Guid("60e51639-59fe-4841-925b-1b80594493b3");
        }
    }

    public override string AuthorName
    {
        get
        {
            //Return a string identifying you or your company.
            return "Microsoft";
        }
    }
    public override string AuthorContact
    {
        get
        {
            //Return a string representing your preferred contact details.
            return "";
        }
    }
}
}
