#pragma checksum "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "57be4d180fcdb8d774ae7a5fde28daaa0efaaa98"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Config_Backgrounds_Grid), @"mvc.1.0.view", @"/Views/Config/Backgrounds/Grid.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Config/Backgrounds/Grid.cshtml", typeof(AspNetCore.Views_Config_Backgrounds_Grid))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/_ViewImports.cshtml"
using InfoScreenPi;

#line default
#line hidden
#line 1 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
using InfoScreenPi.Entities;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"57be4d180fcdb8d774ae7a5fde28daaa0efaaa98", @"/Views/Config/Backgrounds/Grid.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b7aaa20ab4aea634ca8ce3ddabb786700f58a62b", @"/Views/_ViewImports.cshtml")]
    public class Views_Config_Backgrounds_Grid : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<Background>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("add"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/images/addIcon.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(53, 4, true);
            WriteLiteral("    ");
            EndContext();
#line 3 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
      
        for(int i = 0; i <= Model.Count(); i++)
        {
            Background b = null;
            if(i < Model.Count()){
                b = Model.ElementAt(i);
            }
            if(i == 0){

#line default
#line hidden
            BeginContext(264, 16, true);
            WriteLiteral("                ");
            EndContext();
            BeginContext(282, 18, true);
            WriteLiteral("<div class=\"row\">\n");
            EndContext();
#line 12 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
            }
            else if((i % 4) == 0){

#line default
#line hidden
            BeginContext(349, 16, true);
            WriteLiteral("                ");
            EndContext();
            BeginContext(367, 18, true);
            WriteLiteral("<div class=\"row\">\n");
            EndContext();
#line 15 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
            }

            if(i == Model.Count()){

#line default
#line hidden
            BeginContext(436, 16, true);
            WriteLiteral("                ");
            EndContext();
            BeginContext(454, 148, true);
            WriteLiteral("\n                <div class=\"col-xs-6 col-md-3 demo-item\">\n                    <a id=\"linkAddBackground\" class=\"thumbnail\">\n                        ");
            EndContext();
            BeginContext(602, 41, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "634b20a230d4433591e2506dae50c730", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(643, 49, true);
            WriteLiteral("\n                    </a>\n                </div>\n");
            EndContext();
#line 24 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
            }
            else{
                if(b.Url.StartsWith("http://"))
                {

#line default
#line hidden
            BeginContext(790, 20, true);
            WriteLiteral("                    ");
            EndContext();
            BeginContext(812, 89, true);
            WriteLiteral("\n                    <div class=\"col-xs-6 col-md-3 demo-item\">\n                        <a");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 901, "\"", 914, 1);
#line 30 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 908, b.Url, 908, 6, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(915, 52, true);
            WriteLiteral(" class=\"thumbnail\">\n                            <img");
            EndContext();
            BeginWriteAttribute("id", " id=\"", 967, "\"", 977, 1);
#line 31 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 972, b.Id, 972, 5, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginWriteAttribute("src", " src=\"", 978, "\"", 990, 1);
#line 31 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 984, b.Url, 984, 6, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(991, 58, true);
            WriteLiteral(">\n                        </a>\n                    </div>\n");
            EndContext();
#line 34 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
                }
                else
                {
                    string url = "images/backgrounds/" + b.Url;
                    string thumb = "images/backgrounds/thumbnails/" + b.Url;

#line default
#line hidden
            BeginContext(1247, 20, true);
            WriteLiteral("                    ");
            EndContext();
            BeginContext(1269, 89, true);
            WriteLiteral("\n                    <div class=\"col-xs-6 col-md-3 demo-item\">\n                        <a");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 1358, "\"", 1369, 1);
#line 41 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 1365, url, 1365, 4, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(1370, 52, true);
            WriteLiteral(" class=\"thumbnail\">\n                            <img");
            EndContext();
            BeginWriteAttribute("id", " id=\"", 1422, "\"", 1432, 1);
#line 42 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 1427, b.Id, 1427, 5, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginWriteAttribute("src", " src=\"", 1433, "\"", 1445, 1);
#line 42 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
WriteAttributeValue("", 1439, thumb, 1439, 6, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(1446, 58, true);
            WriteLiteral(">\n                        </a>\n                    </div>\n");
            EndContext();
#line 45 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
                }
            }

            if(i == Model.Count()){

#line default
#line hidden
            BeginContext(1573, 16, true);
            WriteLiteral("                ");
            EndContext();
            BeginContext(1591, 7, true);
            WriteLiteral("</div>\n");
            EndContext();
#line 50 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
            }
            else if((((i+1) % 4) == 0) ){

#line default
#line hidden
            BeginContext(1654, 16, true);
            WriteLiteral("                ");
            EndContext();
            BeginContext(1672, 7, true);
            WriteLiteral("</div>\n");
            EndContext();
#line 53 "/home/tvandena/git/infoscreen-pi/InfoScreenPi/Views/Config/Backgrounds/Grid.cshtml"
            }
        }
    

#line default
#line hidden
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Background>> Html { get; private set; }
    }
}
#pragma warning restore 1591
