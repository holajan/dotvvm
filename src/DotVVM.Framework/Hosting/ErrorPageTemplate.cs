// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DotVVM.Framework.Hosting
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class ErrorPageTemplate : ErrorPageTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"
<!DOCTYPE html>
<html>
	<head>
		<title>Server Error in Application</title>
		<style type=""text/css"">
body { font-family: Arial,Tahoma,sans-serif; font-size: 11pt; }
h1 { color: #e00000; font-weight: normal; font-size: 24pt; }
h2 { font-style: normal; font-size: 16pt; font-weight: bold; margin-bottom: 35px; }
h3 { color: #e00000; font-weight: normal; font-size: 14pt; }
pre { background-color: #ffffc0; padding: 20px; font-size: 12pt; }
span.current-line { color: red; }
		</style>
	</head>
	<body>
		<h1>Server Error in Application</h1>
		<h2>HTTP ");
            
            #line 24 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ErrorCode));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 24 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(ErrorDescription)));
            
            #line default
            #line hidden
            this.Write("</h2>\r\n\t\t<p><strong>");
            
            #line 25 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Exception.GetType().FullName));
            
            #line default
            #line hidden
            this.Write(": ");
            
            #line 25 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(Exception.Message)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n\r\n");
            
            #line 27 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (Url != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Request URL: <strong>");
            
            #line 28 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Verb));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 28 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(Url)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 29 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 31 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (ClassName != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Source Class: <strong>");
            
            #line 32 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(ClassName.AssemblyQualifiedName)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 33 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 35 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (!string.IsNullOrEmpty(FileName)) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Source File: <strong>");
            
            #line 36 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(FileName)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 37 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 39 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (LineNumber > 0) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Line: <strong>");
            
            #line 40 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(LineNumber));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 41 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 43 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (!string.IsNullOrEmpty(CurrentUserName)) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Current User: <strong>");
            
            #line 44 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(CurrentUserName)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 45 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 47 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 if (!string.IsNullOrEmpty(IpAddress)) { 
            
            #line default
            #line hidden
            this.Write("\t\t<p>Client IP: <strong>");
            
            #line 48 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(WebUtility.HtmlEncode(IpAddress)));
            
            #line default
            #line hidden
            this.Write("</strong></p>\r\n");
            
            #line 49 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 51 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"
 
	var sourceLines = GetSourceLines();
    if (sourceLines != null)
    {

            
            #line default
            #line hidden
            this.Write("\t\t<p>&nbsp;</p>\r\n\t\t<h3>Source Location</h3>\r\n");
            
            #line 58 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"

        this.Write("<pre>");
        foreach (var line in sourceLines)
        {
            if (line.LineNumber == LineNumber)
            {
                this.Write("<span class='current-line'>");
            }

            if (line.LineNumber == null)
            {
                this.Write(new string(' ', 15));
            }
            else
            {
                this.Write(("Line " + line.LineNumber + ": ").PadLeft(15));
            }
            this.Write(WebUtility.HtmlEncode(line.Text));

			if (line.LineNumber == LineNumber)
            {
                this.Write("</span>");
            }
            this.Write("<br />");
        }
		this.Write("</pre>");
    }

            
            #line default
            #line hidden
            this.Write("\t\t\r\n\t\t<p>&nbsp;</p>\r\n\t\t<h3>Stack Trace</h3>\r\n");
            
            #line 89 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"

    this.Write("<pre>");
    WriteException(Exception);

	if (Exception is ReflectionTypeLoadException) {
		var loaderExceptions = ((ReflectionTypeLoadException)Exception).LoaderExceptions;
		foreach (var ex in loaderExceptions) {
			this.Write("<br />");
			WriteException(ex);
		}
	}

	this.Write("</pre>");

            
            #line default
            #line hidden
            this.Write("</pre>\r\n\t\t<p>&nbsp;</p>\r\n\r\n\t</body>\r\n</html>\r\n\r\n\r\n\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 111 "D:\Riganti\External\DotVVM\src\DotVVM.Framework\Hosting\ErrorPageTemplate.tt"

	public int ErrorCode { get; set; }

	public string ErrorDescription { get; set; }

	public string FileName { get; set; }

	public Type ClassName { get; set; }

	public int LineNumber { get; set; }

	public int PositionOnLine { get; set; }

	public string Url { get; set; }

	public string Verb { get; set; }

	public string IpAddress { get; set; }

	public string CurrentUserName { get; set; }

	public Exception Exception { get; set; }


	private void WriteException(Exception ex) 
	{
		using (var sr = new StringReader(ex.ToString()))
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				this.Write(WebUtility.HtmlEncode(line));
				this.Write("<br />");
			}
		}
	}


	/// <summary>
	/// Gets the source lines near the error and highlights the error.
	/// </summary>
    private IList<SourceLine> GetSourceLines()
	{
	    if (string.IsNullOrEmpty(FileName))
	    {
	        return null;
	    }

        try
        {
            var lines = new List<SourceLine>();
            using (var sr = new StreamReader(FileName, true))
            {
                string line;
                int lineNumber = 0;
                while (!sr.EndOfStream && lineNumber < LineNumber + 2)
                {
                    line = sr.ReadLine();
                    lineNumber++;

                    if (lineNumber >= LineNumber - 2)
                    {
                        // write the line to the output
						lines.Add(new SourceLine() { LineNumber = lineNumber, Text = line });

						// mark the position on the problem line
                        if (lineNumber == LineNumber)
                        {
                            lines.Add(new SourceLine() { Text = new string('-', PositionOnLine) + '^' });
                        }
                    }
                }
            }
            return lines;
        }
        catch
        {
            return null;
        } 
    }


    class SourceLine
    {
        public int? LineNumber { get; set; }

        public string Text { get; set; }
	}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class ErrorPageTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
