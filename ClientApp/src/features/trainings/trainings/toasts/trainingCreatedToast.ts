import { toast } from "sonner";


type TrainingCreatedToastProps = {
    name: string;
}

const trainingCreatedToast = ({ name }: TrainingCreatedToastProps) => {
    toast.success(`${name}`, {
        description: `Training created successfully`,
    });
}

export default trainingCreatedToast;