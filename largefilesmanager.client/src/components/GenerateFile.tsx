import React, { memo, useState } from "react";
import { FileGenerationClient } from "../api/file-generation-api";
import { FileCreatedStatus, FileStatus } from "../models";
import useElapsedTimeFormatter from "../hooks/time-formatter-hook";

const GenerateFile: React.FC = () => {
  const fileSizeOptions = [
    { key: 1, value: "1MB" },
    { key: 5, value: "5MB" },
    { key: 10, value: "10MB" },
    { key: 50, value: "50MB" },
    { key: 100, value: "100MB" },
    { key: 500, value: "500MB" },
    { key: 1000, value: "1G" },
    { key: 5000, value: "5G" },
    { key: 10000, value: "10G" },
    { key: 50000, value: "50G" },
    { key: 100000, value: "100G" },
  ];

  const [status, setStatus] = useState(0);
  const [generating, setGenerating] = useState(false);
  const [fileName, setFileName] = useState("");
  const [selectedFileSize, setSelectedFileSize] = useState(
    fileSizeOptions[0].key
  );
  const [elapsedTime, setElapsedTime] = useState<number | null>(null);

  const formattedTime = useElapsedTimeFormatter(elapsedTime || 0);

  const filesClient = new FileGenerationClient();

  const checkFileStatus = async (fileNameParam: string, startTime: number) => {
    try {
      const data: FileStatus = await filesClient.getFileStatus(fileNameParam);
      setStatus(data.status);

      if (data.status === 100) {
        setGenerating(false);
        const endTime = Date.now();
        if (startTime) {
          const elapsed = endTime - startTime;
          setElapsedTime(elapsed);
        }
        return;
      }

      setTimeout(() => checkFileStatus(fileNameParam, startTime), 1000);
    } catch (error) {
      console.error("Error fetching file status:", error);
      setGenerating(false);
    }
  };

  const handleGenerate = async () => {
    if (generating) {
      return;
    }

    try {
      const now = Date.now();
      setGenerating(true);
      setStatus(0);
      setElapsedTime(null);

      const data: FileCreatedStatus = await filesClient.generateFile(
        selectedFileSize
      );

      setFileName(data.fileName);
      checkFileStatus(data.fileName, now);
    } catch (error) {
      console.error("Error starting file generation:", error);
      setGenerating(false);
    }
  };

  const triggerDownload = async () => {
    try {
      await filesClient.downloadFile(fileName);
    } catch (error) {
      console.error("Error downloading the file:", error);
    }
  };

  const clearStates = async () => {
    setStatus(0);
    setGenerating(false);
    setFileName("");
    setElapsedTime(null);

    await filesClient.deleteFiles();
  };

  return (
    <div>
      <h2>File Generator</h2>

      {!generating && status === 0 && (
        <>
          <div>
            <label htmlFor="fileSize">Select File Size: </label>
            <select
              id="fileSize"
              value={selectedFileSize}
              onChange={(e) => setSelectedFileSize(Number(e.target.value))}
            >
              {fileSizeOptions.map((option) => (
                <option key={option.key} value={option.key}>
                  {option.value}
                </option>
              ))}
            </select>
          </div>
          <div>
            <button onClick={handleGenerate} disabled={generating}>
              Generate File
            </button>
          </div>
        </>
      )}

      {generating && (
        <div>
          <p>Generating file... Progress: {status}%</p>
          <progress
            value={status}
            max="100"
            style={{ width: "100%" }}
          ></progress>
        </div>
      )}

      {status === 100 && (
        <div>
          <p>File {fileName} was generated successfully!</p>
          <p>File was generated in time: {formattedTime}</p>
          <button onClick={triggerDownload}>Download File to view</button>
          <br />
          <button onClick={clearStates}>Clear</button>
        </div>
      )}
    </div>
  );
};

const _GenerateFile = memo(GenerateFile);

export { _GenerateFile as GenerateFile };
