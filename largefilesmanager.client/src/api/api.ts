export abstract class BaseClient {
  protected async get<T>(
    endpoint: string,
    isBlob: boolean = false
  ): Promise<T | Blob> {
    return fetchData<T>(
      endpoint,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      },
      isBlob
    );
  }

  protected async post<T>(
    endpoint: string,
    body: object,
    isBlob: boolean = false
  ): Promise<T | Blob> {
    return fetchData<T>(
      endpoint,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
      },
      isBlob
    );
  }

  protected async delete<T>(endpoint: string): Promise<T | void> {
    const response = await fetch(endpoint, { method: "DELETE" });

    if (!response.ok) {
      const errorMessage = await response.text();
      throw new Error(errorMessage);
    }

    return response.status === 204 ? undefined : response.json();
  }
}

export const fetchData = async <T>(
  endpoint: string,
  config?: RequestInit,
  isBlob: boolean = false
): Promise<T | Blob> => {
  try {
    const response = await fetch(endpoint, config);

    if (!response.ok) {
      const errorMessage = await response.text();
      throw new Error(errorMessage);
    }

    if (isBlob) {
      return await response.blob();
    }

    return (await response.json()) as T;
  } catch (err) {
    console.error("Fetch Error:", err);
    throw new Error(
      err instanceof Error ? err.message : "An unexpected error occurred."
    );
  }
};

export const getRequestOptions = (): RequestInit => ({
  method: "GET",
  headers: {
    "Content-Type": "application/json",
  },
});

export const postRequestOptions = (body: object): RequestInit => ({
  method: "POST",
  headers: {
    "Content-Type": "application/json",
  },
  body: JSON.stringify(body),
});
