using System;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Theming;

/// <summary>Refresh icons after native background painting, before image painting.</summary>
internal sealed class MenuImageRefresh : IDisposable
{
    private readonly ToolStrip _strip;
    private readonly Action<ToolStripItem> _refresh;
    private ToolStripRenderer _renderer;

    internal MenuImageRefresh(ToolStrip strip, Action<ToolStripItem> refresh)
    {
        _strip = strip;
        _refresh = refresh;
        strip.RendererChanged += RendererChanged;
        strip.Disposed += Disposed;
        RendererChanged(null, EventArgs.Empty);
    }

    private void RendererChanged(object sender, EventArgs e)
    {
        Detach();
        _renderer = _strip.Renderer;
        _renderer.RenderButtonBackground += Background;
        _renderer.RenderMenuItemBackground += Background;
        _renderer.RenderDropDownButtonBackground += Background;
        _renderer.RenderSplitButtonBackground += Background;
        _renderer.RenderLabelBackground += Background;
    }

    private void Background(object sender, ToolStripItemRenderEventArgs e)
    {
        // Renderers may be shared by unrelated menus. Never recolor their assets.
        if (e.Item.Owner == _strip) _refresh(e.Item);
    }

    private void Detach()
    {
        if (_renderer == null) return;
        _renderer.RenderButtonBackground -= Background;
        _renderer.RenderMenuItemBackground -= Background;
        _renderer.RenderDropDownButtonBackground -= Background;
        _renderer.RenderSplitButtonBackground -= Background;
        _renderer.RenderLabelBackground -= Background;
        _renderer = null;
    }

    private void Disposed(object sender, EventArgs e) => Dispose();
    public void Dispose()
    {
        _strip.RendererChanged -= RendererChanged;
        _strip.Disposed -= Disposed;
        Detach();
    }
}
