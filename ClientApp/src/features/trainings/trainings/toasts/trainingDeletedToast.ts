import { toast } from "sonner";


type TrainingDeletedToastProps = {
    name: string;
}

const trainingDeletedToast = ({ name }: TrainingDeletedToastProps) => {
    toast.success(`${name}`, {
        description: `Training deleted successfully`,
    });
}

export default trainingDeletedToast;
