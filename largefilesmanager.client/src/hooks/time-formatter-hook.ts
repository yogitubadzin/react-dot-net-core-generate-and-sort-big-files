import { useMemo } from "react";

const useElapsedTimeFormatter = (elapsed: number) => {
  const formattedTime = useMemo(() => {
    const seconds = Math.floor((elapsed / 1000) % 60);
    const minutes = Math.floor((elapsed / (1000 * 60)) % 60);
    const hours = Math.floor((elapsed / (1000 * 60 * 60)) % 24);

    return `${hours.toString().padStart(2, "0")}:${minutes
      .toString()
      .padStart(2, "0")}:${seconds.toString().padStart(2, "0")}`;
  }, [elapsed]);

  return formattedTime;
};

export default useElapsedTimeFormatter;
