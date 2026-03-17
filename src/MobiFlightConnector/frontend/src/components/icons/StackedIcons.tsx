import React from "react";

type StackedIconsProps = {
  bottomIcon: React.ReactNode; // The bottom icon
  topIcon: React.ReactNode;   // The top icon
  className?: string;         // Optional additional class names
};

const StackedIcons = ({ bottomIcon, topIcon, className }: StackedIconsProps) => {
  return (
    <div className={`relative inline-block w-6 h-6 ${className}`}>
      {/* Bottom icon */}
      <div className="absolute inset-0">{bottomIcon}</div>
      {/* Top icon */}
      <div className="absolute inset-0">{topIcon}</div>
    </div>
  );
};

export default StackedIcons;