using System;
using System.Windows.Forms;

namespace ProjectManager
{
    internal sealed class DefaultRenderer : ToolStripProfessionalRenderer
    {
        public static readonly DefaultRenderer Instance = new DefaultRenderer();

        private DefaultRenderer()
        {
            RoundedEdges = false;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
        }
    }
}
