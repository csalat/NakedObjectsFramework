﻿// Copyright 2013-2014 Naked Objects Group Ltd
// Licensed under the Apache License, Version 2.0(the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

module NakedObjects {

    // custom configuration for a particular implementation 

    // Specify path to Restful Objects server 
    const appPath = "[No Server Name Specified]" // e.g. "http://localhost:12345";

    export function getAppPath() {
        if (appPath.charAt(appPath.length - 1) === "/") {
            return appPath.length > 1 ? appPath.substring(0, appPath.length - 1) : "";
        }
        return appPath;
    }


    export const defaultPageSize = 20; // can be overriden by server 
    export const listCacheSize = 5;

    export const shortCutMarker = "___";
	//Add strings that appear frequently in URLs -  to automatically compress them
    export const urlShortCuts = [];

    export const keySeparator = "--";
    export const objectColor = "object-color";
    export const linkColor = "link-color";

    export const autoLoadDirty = true;
    export const showDirtyFlag = false || !autoLoadDirty;
}