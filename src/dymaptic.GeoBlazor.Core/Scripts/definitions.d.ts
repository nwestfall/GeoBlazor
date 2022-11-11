﻿import Layer from "@arcgis/core/layers/Layer";

export interface MapObject {
    destroy();
    declaredClass: string;

    on(eventName: string, callback: (evt) => any): void;
}

export interface MapCollection extends __esri.Collection {
    items: any[];
}

export interface DotNetGraphic {
    uid: string;
    geometry: any;
}

export interface DotNetFeature {
    uid: string;
    geometry: any;
    attributes: any;
}

export interface DotNetGeometry {
    type: string;
    spatialReference: DotNetSpatialReference;
}

export interface DotNetPoint extends DotNetGeometry {
    latitude: number;
    longitude: number;
    hasM: boolean;
    hasZ: boolean;
    extent: DotNetExtent;
    x: number;
    y: number;
}

export interface DotNetExtent extends DotNetGeometry {
    xmin: number;
    ymin: number;
    xmax: number;
    ymax: number;
    zmin: number;
    zmax: number;
    mmin: number;
    mmax: number;
    hasM: boolean;
    hasZ: boolean;
}

export interface DotNetPolygon extends DotNetGeometry {
    rings: number[][][];
    hasM: boolean;
    hasZ: boolean;
    extent: DotNetExtent,
}

export interface DotNetPolyline extends DotNetGeometry {
    paths: number[][][];
    hasM: boolean;
    hasZ: boolean;
    extent: DotNetExtent;
}

export interface DotNetSpatialReference {
    isGeographic: boolean;
    isWebMercator: boolean;
    isWgs84: boolean;
    isWrappable: boolean;
    wkid: number;
    wkt: string;
    imageCoordinateSystem: any;
}

export interface DotNetGeographicTransformation {
    steps: Array<DotNetGeographicTransformationStep>
}

export interface DotNetGeographicTransformationStep {
    isInverse: boolean;
    wkid: number;
    wkt: string
}

export interface DotNetActionSection {
    title: string,
    className: string,
    id: string
}

export interface DotNetListItem {
    title: string;
    layer: Layer;
    visible: boolean;
    children: DotNetListItem[],
    actionSections: DotNetActionSection[][]
}