// Set up the .NET WebAssembly runtime
import { dotnet } from './_framework/dotnet.js'

// Get exported methods from the .NET assembly
const { getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

window.dotnetapp = exports.Ares.Webasm.LanguageHost;
console.log('Loaded .NET application...');
const event = new Event("dotnetinitialized");
window.dispatchEvent(event);