import { getOrderStatusMeta, getPaymentStatusMeta, normalizeOrderStatus } from "@/lib/utils";
import { OrderStatus, PaymentStatus } from "@/types";

type Props =
  | { status: OrderStatus | string | number; type: "order" }
  | { status: PaymentStatus | string; type: "payment" };

export default function StatusBadge({ status, type }: Props) {
  const meta =
    type === "order"
      ? getOrderStatusMeta(normalizeOrderStatus(status))
      : getPaymentStatusMeta(status as string);

  return (
    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${meta.badge}`}>
      {meta.label}
    </span>
  );
}
