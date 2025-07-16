type ApiSuccess<T> = {
  success: true;
  data: T;
};

type ApiError = {
  success: false;
  error: string;
  data?: any;
};

export type ApiResponse<T> = ApiSuccess<T> | ApiError;