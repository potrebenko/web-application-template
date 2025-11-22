// Configuration class with type safety
export class AppConfig {
  private static instance: AppConfig;
  private _appTitle: string = 'Net Template';
  private _apiUrl: string = 'http://localhost:5214';
  private _version: string = '1.0.0';
  private _isDevelopment: boolean = import.meta.env.DEV;

  private constructor() {
    // Load from environment variables if available
    if (import.meta.env.VITE_APP_TITLE) {
      this._appTitle = import.meta.env.VITE_APP_TITLE;
    }
    
    if (import.meta.env.VITE_API_URL) {
      this._apiUrl = import.meta.env.VITE_API_URL;
    }
  }

  public static getInstance(): AppConfig {
    if (!AppConfig.instance) {
      AppConfig.instance = new AppConfig();
    }
    return AppConfig.instance;
  }

  get appTitle(): string {
    return this._appTitle;
  }

  get apiUrl(): string {
    return this._apiUrl;
  }

  get version(): string {
    return this._version;
  }

  get isDevelopment(): boolean {
    return this._isDevelopment;
  }
}

// Export a singleton instance
export default AppConfig.getInstance();
