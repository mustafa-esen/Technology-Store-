interface Props {
  label: string;
  badgeClass: string;
}

export function StatusBadge({ label, badgeClass }: Props) {
  return (
    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${badgeClass}`}>
      {label}
    </span>
  );
}
