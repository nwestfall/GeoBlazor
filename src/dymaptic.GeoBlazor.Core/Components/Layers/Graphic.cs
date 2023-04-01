﻿using dymaptic.GeoBlazor.Core.Components.Geometries;
using dymaptic.GeoBlazor.Core.Components.Popups;
using dymaptic.GeoBlazor.Core.Components.Symbols;
using dymaptic.GeoBlazor.Core.Components.Views;
using dymaptic.GeoBlazor.Core.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProtoBuf;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace dymaptic.GeoBlazor.Core.Components.Layers;

/// <summary>
///     A Graphic is a vector representation of real world geographic phenomena. It can contain geometry, a symbol, and
///     attributes. A Graphic is displayed in the GraphicsLayer.
///     <a target="_blank" href="https://developers.arcgis.com/javascript/latest/api-reference/esri-Graphic.html">
///         ArcGIS JS
///         API
///     </a>
/// </summary>
public class Graphic : LayerObject, IEquatable<Graphic>
{
    /// <summary>
    ///     Parameterless constructor for using as a razor component
    /// </summary>
    public Graphic()
    {
    }

    /// <summary>
    ///     Constructs a new Graphic in code with parameters
    /// </summary>
    /// <param name="geometry">
    ///     The geometry that defines the graphic's location.
    /// </param>
    /// <param name="symbol">
    ///     The <see cref="Symbol" /> for the object.
    /// </param>
    /// <param name="popupTemplate">
    ///     The <see cref="PopupTemplate" /> for displaying content in a Popup when the graphic is selected.
    /// </param>
    /// <param name="attributes">
    ///     Name-value pairs of fields and field values associated with the graphic.
    /// </param>
    public Graphic(Geometry? geometry = null, Symbol? symbol = null, PopupTemplate? popupTemplate = null,
        Dictionary<string, object>? attributes = null)
    {
        AllowRender = false;
#pragma warning disable BL0005
        Geometry = geometry;
        Symbol = symbol;
        PopupTemplate = popupTemplate;

        if (attributes != null)
        {
            Attributes = attributes;
        }
#pragma warning restore BL0005
        ToSerializationRecord();
    }

    /// <summary>
    ///     Compares two <see cref="Graphic" /> instances for equality.
    /// </summary>
    public static bool operator ==(Graphic? left, Graphic? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    ///     Compares two <see cref="Graphic" /> instances for inequality.
    /// </summary>
    public static bool operator !=(Graphic? left, Graphic? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    ///     Name-value pairs of fields and field values associated with the graphic.
    /// </summary>
#pragma warning disable BL0007
    [Parameter]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Attributes
    {
        get => _attributes;
        set => _attributes = value is null
            ? new ObservableDictionary<string, object>()
            : new ObservableDictionary<string, object>(value);
    }
#pragma warning restore BL0007

    /// <summary>
    ///     The geometry that defines the graphic's location.
    /// </summary>
    /// <remarks>
    ///     To retrieve a current geometry for a graphic, use <see cref="GetGeometry" /> instead of calling this Property
    ///     directly.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonInclude]
    public Geometry? Geometry { get; private set; }

    /// <summary>
    ///     The <see cref="PopupTemplate" /> for displaying content in a Popup when the graphic is selected.
    /// </summary>
    /// <remarks>
    ///     To retrieve a current popup template for a graphic, use <see cref="GetPopupTemplate" /> instead of calling this
    ///     Property directly.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonInclude]
    public PopupTemplate? PopupTemplate { get; private set; }

    /// <summary>
    ///     The GeoBlazor Id of the parent layer, used when serializing the graphic to/from JavaScript.
    /// </summary>
    public Guid? LayerId { get; set; }

    /// <inheritdoc />
    public bool Equals(Graphic? other)
    {
        return (other?.Id == Id) ||
            (other is not null &&
                (other.Geometry?.Equals(Geometry) == true) &&
                (other.Attributes?.Equals(Attributes) == true));
    }

    /// <summary>
    ///     Retrieves the <see cref="Geometry" /> from the rendered graphic.
    /// </summary>
    public async Task<Geometry?> GetGeometry()
    {
        if (JsModule is not null)
        {
            Geometry = await JsModule!.InvokeAsync<Geometry>("getGraphicGeometry",
                CancellationTokenSource.Token, Id);
        }

        return Geometry;
    }

    /// <summary>
    ///     Sets the <see cref="Geometry" /> on the rendered graphic.
    /// </summary>
    /// <param name="geometry"></param>
    public async Task SetGeometry(Geometry geometry)
    {
        Geometry = geometry;

        if (JsModule is not null)
        {
            await JsModule.InvokeVoidAsync("setGraphicGeometry", Id, 
                Geometry.ToSerializationRecord());
        }
        else
        {
            _updateGeometry = true;
        }

        _serializationRecord = null;
        ToSerializationRecord();
    }

    /// <inheritdoc />
    public override async Task SetSymbol(Symbol symbol)
    {
        await base.SetSymbol(symbol);
        _serializationRecord = null;
        ToSerializationRecord();
    }

    /// <summary>
    ///     Retrieves the <see cref="PopupTemplate" /> from the rendered graphic.
    /// </summary>
    public async Task<PopupTemplate?> GetPopupTemplate()
    {
        if (JsModule is not null)
        {
            PopupTemplate = await JsModule!.InvokeAsync<PopupTemplate>("getGraphicPopupTemplate",
                CancellationTokenSource.Token, Id);
        }

        return PopupTemplate;
    }

    /// <summary>
    ///     Sets the <see cref="PopupTemplate" /> on the rendered graphic.
    /// </summary>
    /// <param name="popupTemplate">
    ///     The <see cref="PopupTemplate" /> for displaying content in a Popup when the graphic is selected.
    /// </param>
    public async Task SetPopupTemplate(PopupTemplate popupTemplate)
    {
        PopupTemplate = popupTemplate;

        if (JsModule is not null)
        {
            await JsModule.InvokeVoidAsync("setGraphicPopupTemplate", Id,
                PopupTemplate.ToSerializationRecord(), View?.Id);
        }
        else
        {
            _updatePopupTemplate = true;
        }
        
        _serializationRecord = null;
        ToSerializationRecord();
    }

    /// <inheritdoc />
    public override async Task<Symbol?> GetSymbol()
    {
        if (JsModule is not null)
        {
            Symbol = await JsModule!.InvokeAsync<Symbol>("getGraphicSymbol",
                CancellationTokenSource.Token, Id);
        }

        return Symbol;
    }

    /// <inheritdoc />
    public override async Task RegisterChildComponent(MapComponent child)
    {
        switch (child)
        {
            case Geometry geometry:
                if (View?.ExtentChangedInJs == true)
                {
                    return;
                }
                
                await SetGeometry(geometry);

                break;
            case PopupTemplate popupTemplate:
                if (View?.ExtentChangedInJs == true)
                {
                    return;
                }
                
                await SetPopupTemplate(popupTemplate);

                break;
            default:
                await base.RegisterChildComponent(child);

                break;
        }

        ToSerializationRecord(true);
    }

    /// <inheritdoc />
    public override async Task UnregisterChildComponent(MapComponent child)
    {
        switch (child)
        {
            case Geometry _:
                Geometry = null;

                break;
            case PopupTemplate _:
                PopupTemplate = null;

                break;
            default:
                await base.UnregisterChildComponent(child);

                break;
        }
    }

    /// <inheritdoc />
    public override void ValidateRequiredChildren()
    {
        base.ValidateRequiredChildren();
        Geometry?.ValidateRequiredChildren();
        PopupTemplate?.ValidateRequiredChildren();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Graphic)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        if (JsModule is not null)
        {
            await JsModule.InvokeVoidAsync("disposeGraphic", Id);
        }
    }

    internal GraphicSerializationRecord ToSerializationRecord(bool refresh = false)
    {
        if (_serializationRecord is null || refresh)
        {
            _serializationRecord = new(Id.ToString(),
                Geometry?.ToSerializationRecord(),
                Symbol?.ToSerializationRecord(),
                PopupTemplate?.ToSerializationRecord(),
                Attributes?.Select(kvp =>
                        new AttributeSerializationRecord(kvp.Key, kvp.Value.ToString(), kvp.Value.GetType().ToString()))
                    .ToArray());
        }

        return _serializationRecord;
    }


    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _attributes.CollectionChanged -= OnAttributesChanged;
        _attributes.CollectionChanged += OnAttributesChanged;
        ToSerializationRecord(true);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (JsModule is not null)
        {
            if (UpdateSymbol)
            {
                UpdateSymbol = false;
                await SetSymbol(Symbol!);
            }

            if (_updateGeometry)
            {
                _updateGeometry = false;
                await SetGeometry(Geometry!);
            }
            
            if (_updatePopupTemplate)
            {
                _updatePopupTemplate = false;
                await SetPopupTemplate(PopupTemplate!);
            }
        }
    }

    private async void OnAttributesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (JsModule is null) return;

        await JsModule.InvokeVoidAsync("setGraphicAttributes", 
            CancellationTokenSource.Token, Id, Attributes);
        ToSerializationRecord(true);
    }
    
    private ObservableDictionary<string, object> _attributes = new();
    private GraphicSerializationRecord? _serializationRecord;
    private bool _updateGeometry;
    private bool _updatePopupTemplate;
}

[ProtoContract(Name = "Graphic")]
internal record GraphicSerializationRecord([property:ProtoMember(1)]string Id, 
    [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [property:ProtoMember(2)]GeometrySerializationRecord? Geometry, 
    [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [property:ProtoMember(3)]SymbolSerializationRecord? Symbol, 
    [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [property:ProtoMember(4)]PopupTemplateSerializationRecord? PopupTemplate,
    [property:ProtoMember(5)]AttributeSerializationRecord[]? Attributes)
    : MapComponentSerializationRecord;
    
[ProtoContract(Name = "Attribute")]
internal record AttributeSerializationRecord([property:ProtoMember(1)]string Key, 
    [property:ProtoMember(2)]string? Value, [property:ProtoMember(3)]string ValueType);