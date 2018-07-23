// Expose an export called 'platform' of the interface type 'Platform',
// so that consumers can be agnostic about which implementation they use.
// Basic alternative to having an actual DI container.
import { Platform } from './Platform/Platform';
import { exportBlazor } from './GlobalExports';

export let platform: Platform;

export function setPlatform(platformInstance: Platform) {
  platform = platformInstance;
  exportBlazor();
  return platform;
}
