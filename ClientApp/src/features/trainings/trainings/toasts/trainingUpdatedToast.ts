import { toast } from "sonner";


type TrainingUpdatedToastProps = {
    name: string;
}

const trainingUpdatedToast = ({ name }: TrainingUpdatedToastProps) => {
    toast.success(`${name}`, {
        description: `Training updated successfully`,
    });
}

export default trainingUpdatedToast;